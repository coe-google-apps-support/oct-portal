using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using COE_WOI_WorkOrderInterface_WS;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy
{
    public class RemedyService : IRemedyService
    {
        public RemedyService(New_Port_0PortTypeClient remedyClient)
        {
            _remedyClient = remedyClient;
        }

        private readonly New_Port_0PortTypeClient _remedyClient;



        public async Task PostNewIdeaAsync(Idea idea, string user3and3)
        {

            var request = new New_Create_Operation_0Request();

            AuthenticationInfo authInfo = new AuthenticationInfo();
            authInfo.userName = ""; // give a valid AR user name 
            authInfo.password = ""; // give a valid password 


            using (new OperationContextScope(_remedyClient.InnerChannel))
            {
                try
                {
                    await _remedyClient.New_Create_Operation_0Async(authInfo,
                        Customer_Company: "City of Edmonton",
                        Customer_Login_ID: "",
                        z1D_Action: "Create",
                        Summary: idea.Title,
                        Description: idea.Description,
                        Requested_For: Requested_ForType.Individual,
                        Location_Company: "City of Edmonton",
                        Work_Order_Template_Used: "Work Order - Generic - Create",
                        TemplateName: "Work Order - Generic - Create");
                }
                catch (Exception err)
                {

                    throw;
                }
            }

        }
    }
}
