using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using COE_WOI_WorkOrderInterface_WS;
using Microsoft.Extensions.Options;
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


        public async Task PostNewIdeaAsync(Idea idea, WordPressUser user, string user3and3)
        {
            var request = new New_Create_Operation_0Request();

            AuthenticationInfo authInfo = new AuthenticationInfo();
            authInfo.userName = _options.ServiceUserName; // give a valid AR user name 
            authInfo.password = _options.ServicePassword; // give a valid password 
            
            try
            {
               var response = await _remedyClient.New_Create_Operation_0Async(authInfo,
                    Customer_Login_ID: user3and3,
                    Summary: idea.Title,
                    Description: idea.Description,
                    Requested_For: Requested_ForType.Individual,
                    Location_Company: _options.LocationCompany,
                    Customer_Company: _options.CustomerCompany,
                    TemplateId: _options.TemplateId,
                    Customer_First_Name: user.FirstName,
                    Customer_Last_Name: user.LastName,
                    Categorization_Tier_1: _options.CategorizationTier1,
                    Categorization_Tier_2: _options.CategorizationTier2);
            }
            catch (Exception err)
            {
                Trace.TraceError($"Unable to create ticket in Remedy: { err.Message }");
                throw;
            }

        }
    }
}
