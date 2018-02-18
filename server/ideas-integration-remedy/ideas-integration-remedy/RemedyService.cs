using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy.RemedyServiceReference;
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
        public RemedyService(New_Port_0PortType remedyClient,
            IOptions<RemedyServiceOptions> options)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            if (options == null)
                throw new ArgumentNullException("options");
            _options = options?.Value;
        }

        private readonly New_Port_0PortType _remedyClient;
        private readonly RemedyServiceOptions _options;


        public async Task<string> PostNewIdeaAsync(Idea idea, string user3and3)
        {
            try
            {
                var request = new New_Create_Operation_0Request(
                    AuthenticationInfo: new AuthenticationInfo()
                    {
                        userName = _options.ServiceUserName,
                        password = _options.ServicePassword
                    },
                    Customer_Company: _options.CustomerCompany,
                    Customer_Login_ID: _options.CustomerLoginId,
                    z1D_Action: "Create",
                    Summary: idea.Title,
                    Description: idea.Description,
                    Requested_For: Requested_ForType.Individual,
                    Location_Company: _options.LocationCompany,
                    Work_Order_Template_Used: _options.WorkOrderTemplateUsed,
                    TemplateName: _options.TemplateName,
                    Categorization_Tier_1: _options.CategorizationTier1,
                    Categorization_Tier_2: _options.CategorizationTier2,
                    Categorization_Tier_3: _options.CategorizationTier3,
                    TemplateID: _options.TemplateId,
                    Customer_First_Name: _options.CustomerFirstName,
                    Customer_Last_Name: _options.CustomerLastName);

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
