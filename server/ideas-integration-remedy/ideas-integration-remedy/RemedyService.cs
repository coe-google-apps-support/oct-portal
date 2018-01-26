using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Options;
using RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy
{
    public class RemedyService : IRemedyService
    {
        public RemedyService(New_Port_0PortTypeClient remedyClient,
            IOptions<RemedyServiceOptions> options)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            if (options == null)
                throw new ArgumentNullException("options");
            _options = options?.Value;
        }

        private readonly New_Port_0PortTypeClient _remedyClient;
        private readonly RemedyServiceOptions _options;


        public async Task<string> PostNewIdeaAsync(Idea idea, WordPressUser user, string user3and3)
        {
            var request = new New_Create_Operation_0Request()
            {
                AuthenticationInfo = new AuthenticationInfo()
                {
                    userName = _options.ServiceUserName,
                    password = _options.ServicePassword
                },
                Customer_Login_ID = _options.CustomerLoginId,
                Summary = idea.Title,
                //Description = idea.Description, // should this be Long_Description?
                Location_Company = _options.LocationCompany,
                Customer_Company = _options.CustomerCompany,
                TemplateID = _options.TemplateId,
                Customer_First_Name = _options.CustomerFirstName,
                Customer_Last_Name = _options.CustomerLastName,
                Categorization_Tier_1 = _options.CategorizationTier1,
                Categorization_Tier_2 = _options.CategorizationTier2,
                z1D_Action = "Create"
            };


            try
            {
                var response = await _remedyClient.New_Create_Operation_0Async(request);
                return response.InstanceId;
            }
            catch (Exception err)
            {
                Trace.TraceError($"Unable to create ticket in Remedy: { err.Message }");
                throw;
            }

        }
    }
}
