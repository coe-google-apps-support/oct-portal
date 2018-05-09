using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Server.Controllers;
using CoE.Ideas.Server.Models;
using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.WordPress;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Apex
{
    internal class ApexListener
    {
        public ApexListener(IInitiativeRepository initiativeRepository,
                            Serilog.ILogger logger, 
                            IPeopleService peopleService, 
                            IPersonRepository userRepository, 
                            IdeasController ideasController,
                            IWordPressUserSecurity wordPressUserSecurity,
                            IHttpContextAccessor httpContextAccessor,
                            IOptions<ApexOptions> options)
        {
            //using (var con = new Oracle.ManagedDataAccess.Client.OracleConnection("SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=orarac2-scan.gov.edmonton.ab.ca)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ACES12R1.GOV.EDMONTON.AB.CA)));uid = ITO_RO; pwd = C5dzfAWeegB1; "))
            _initiativeRepository = initiativeRepository;
            _logger = logger;
            _peopleService = peopleService;
            _userRepository = userRepository;
            _ideasController = ideasController;
            _wordPressUserSecurity = wordPressUserSecurity;
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        private readonly IInitiativeRepository _initiativeRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IPeopleService _peopleService;
        private readonly IPersonRepository _userRepository;
        private readonly IdeasController _ideasController;
        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly IOptions<ApexOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public async Task Read()
        {
            using (var con = new Oracle.ManagedDataAccess.Client.OracleConnection(_options.Value.ApexConnectionString))
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT BTIR_ID, BTIR_CREATED_BY, BTIR_PROJECT_NAME, BTIR_PROJECT_DESC FROM OCI.BTIR_REQUEST";
                con.Open();
                var reader = cmd.ExecuteReader();
                //IdeasController ideasController = new IdeasController();

                while (reader.Read())
                {
                    int apexId = reader.GetInt32(0);

                    // check to ensure we don't create the initiatives twice
                    if ((await _initiativeRepository.GetInitiativeByApexId(apexId)) != null)
                        continue;

                    string user3and3 = reader.GetString(1);
                    PersonData userInfo;
                    try { userInfo = await _peopleService.GetPersonAsync(user3and3); }
                    catch (Exception err)
                    {
                        // what to do?
                        _logger.Error("User not found for id {UserId}: {ErrorMessage}", user3and3, err.Message);
                        userInfo = null;
                        continue;
                    } 

                    var userId = await _userRepository.GetPersonIdByEmailAsync(userInfo.Email);
                    if (userId == null)
                    {
                        var newUserInfo = await _userRepository.CreatePerson(userInfo.GivenName, userInfo.Surname, userInfo.Email, userInfo.Telephone);
                        userId = newUserInfo.Id;
                    }

                    var newInitiative = await CreateInitiative(reader.GetString(2), reader.GetString(3), userId.Value);
                    if (newInitiative == null)
                        _logger.Error("Created Initiative but controller returned null");
                    else
                    {
                        newInitiative.SetApexId(apexId);
                        await _initiativeRepository.UpdateInitiativeAsync(newInitiative);
                    }
                }

                con.Close();

            }
        }

        private async Task<Initiative> CreateInitiative(string title, string description, int userId)
        {
            var httpContext = new DefaultHttpContext
            {
                User = await _wordPressUserSecurity.GetPrincipalAsync(userId)
            };
            _ideasController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = httpContext
            };
            _httpContextAccessor.HttpContext = httpContext;


            var result = await _ideasController.PostInitiative(new AddInitiativeDto() { Title = title, Description = description }, true);

            var objectResult = result as ObjectResult;
            if (objectResult != null)
                return objectResult.Value as Initiative;
            else
                return null;

        }
    }
}