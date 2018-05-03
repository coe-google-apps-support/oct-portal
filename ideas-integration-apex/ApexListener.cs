using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Server.Controllers;
using CoE.Ideas.Server.Models;
using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Apex
{
    internal class ApexListener
    {
        public ApexListener(IInitiativeRepository initiativeRepository,
                            Serilog.ILogger logger,
                            IPeopleService peopleService,
                            IPersonRepository userRepository,
                            IWordPressUserSecurity wordPressUserSecurity,
                            IHttpContextAccessor httpContextAccessor,
                            IOptions<ApexOptions> options)
        {
            //using (var con = new Oracle.ManagedDataAccess.Client.OracleConnection("SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=orarac2-scan.gov.edmonton.ab.ca)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ACES12R1.GOV.EDMONTON.AB.CA)));uid = ITO_RO; pwd = C5dzfAWeegB1; "))
            _initiativeRepository = initiativeRepository;
            _logger = logger;
            _peopleService = peopleService;
            _userRepository = userRepository;
            _wordPressUserSecurity = wordPressUserSecurity;
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        private readonly IInitiativeRepository _initiativeRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IPeopleService _peopleService;
        private readonly IPersonRepository _userRepository;
        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly IOptions<ApexOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public async Task Read()
        {
            try
            {
                await ReadApexItemsAsync(async (apexItem, cancellationToken) =>
                {
                    // Don't create duplicate Octava records. In the future we can update instead of quitting here
                    if ((await _initiativeRepository.GetInitiativeByApexId(apexItem.ApexId)) != null)
                        return;

                    // Convert apex item creator (in 3+3 format) to email address and then get their Octava user id
                    string requestorEmail = await GetEmailForUser3and3Async(apexItem.Requestor3and3);
                    int? userId;
                    try
                    {
                        userId = string.IsNullOrWhiteSpace(requestorEmail)
                      ? null : await GetOrCreateUserIdAsync(requestorEmail, cancellationToken);
                    }
                    catch (Exception err)
                    {
                        userId = null;
                        _logger.Warning(err, "Error retrieving user details for {EmailAddress}", requestorEmail);
                    }


                    // Convert apex item business contact to Octava user id
                    int? busContactUserId;
                    try
                    {
                        busContactUserId = string.Equals(apexItem.BusinessContactEmail, requestorEmail, StringComparison.InvariantCultureIgnoreCase)
                      ? userId : await GetOrCreateUserIdAsync(apexItem.BusinessContactEmail, cancellationToken);
                    }
                    catch (Exception err)
                    {
                        busContactUserId = null;
                        _logger.Warning(err, "Error retrieving user details for {EmailAddress}", apexItem.BusinessContactEmail);
                    }


                    // in case we couldn't find the creator but we did find a business contact, use that instead
                    if (!userId.HasValue && busContactUserId.HasValue)
                        userId = busContactUserId;

                    // abort it we couldn't find an appropriate stakeholder - Octava requires at least one
                    if (!userId.HasValue)
                    {
                        _logger.Warning("Could not find an appropriate user for apex id {ApexId} by submittor {User3and3} with project title {ApexProjectName}",
                            apexItem.ApexId, apexItem.Requestor3and3, apexItem.ProjectName);
                        return;
                    }

                    // Create initiative
                    var newInitiative = await CreateInitiativeAsync(apexItem.ProjectName,
                        apexItem.ProjectDescription, userId.Value, busContactUserId, apexItem.ApexId, cancellationToken);

                    if (newInitiative == null)
                        _logger.Error("Created Initiative but repository returned null");
                    else
                        _logger.Information("Created initiative {InitiativeId} from APEX item {ApexId} with title {Title}", newInitiative.Id, apexItem.ApexId, newInitiative.Title);
                });
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error sychronizing APEX items to Octava: {ErrorMessage}", err.Message);
                throw;
            }
        }

        private async Task ReadApexItemsAsync(Func<ApexItemDto, CancellationToken, Task> onReadItemAsync,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureArg.IsNotNull(onReadItemAsync);
                
            using (var con = new Oracle.ManagedDataAccess.Client.OracleConnection(_options.Value.ApexConnectionString))
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT BTIR_ID, BTIR_CREATED_BY, BTIR_BUSS_CONTACT_EMAIL, BTIR_PROJECT_NAME, BTIR_PROJECT_DESC FROM OCI.BTIR_REQUEST";
                con.Open();
                var reader = cmd.ExecuteReader();

                try
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        await onReadItemAsync(new ApexItemDto()
                        {
                            ApexId = reader.GetInt32(0),
                            Requestor3and3 = reader.GetString(1),
                            BusinessContactEmail = reader.GetString(2),
                            ProjectName = reader.GetString(3),
                            ProjectDescription = reader.GetString(4)
                        }, cancellationToken);
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }

        // note that the services this method calls are not cancellable, hence no cancellationToken.
        private async Task<string> GetEmailForUser3and3Async(string user3and3)
        {
            try
            {
                PersonData userInfo = await _peopleService.GetPersonAsync(user3and3);
                if (userInfo == null)
                    _logger.Error("User not found for id {UserId}", user3and3);
                else if (string.IsNullOrWhiteSpace(userInfo.Email))
                    _logger.Error("User found for id {UserId} but email was empty", user3and3);
                else
                    return userInfo.Email;
            }
            catch (Exception err)
            {
                // what to do?
                _logger.Error("User not found for id {UserId}: {ErrorMessage}", user3and3, err.Message);
            }
            return null;

        }

        private async Task<int?> GetOrCreateUserIdAsync(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            
            int? userId = await _userRepository.GetPersonIdByEmailAsync(email);



            if (userId == null || userId <= 0)
            {
                PersonData userInfo = await _peopleService.GetPersonByEmailAsync(email);
                if (userInfo == null)
                {
                    _logger.Error("User not found with email {EmailAddress}", email);
                }
                else
                {
                    _logger.Information("Creating new WordPress user '{UserName}' with email {EmailAddress}", userInfo.DisplayName, email);
                    var newUserInfo = await _userRepository.CreatePerson(userInfo.GivenName, userInfo.Surname, email, userInfo.Telephone, cancellationToken);
                    userId = newUserInfo.Id;
                }
            }

            return userId;
        }

        private async Task<Initiative> CreateInitiativeAsync(string title, 
            string description, 
            int userId, 
            int? busContactUserId, 
            int apexId,
            CancellationToken cancellationToken)
        {
            var httpContext = new DefaultHttpContext
            {
                User = await _wordPressUserSecurity.GetPrincipalAsync(userId)
            };
            _httpContextAccessor.HttpContext = httpContext;

            var newInitiative = Initiative.Create(title, description, userId, busContactUserId, skipEmailNotification: true);
            newInitiative.SetApexId(apexId);
            return await _initiativeRepository.AddInitiativeAsync(newInitiative, cancellationToken);
        }

        private struct ApexItemDto
        {
            public int ApexId { get; set; }
            public string Requestor3and3 { get; set; }
            public string BusinessContactEmail { get; set; }
            public string ProjectName { get; set; }
            public string ProjectDescription { get; set; }
        }

    }
}