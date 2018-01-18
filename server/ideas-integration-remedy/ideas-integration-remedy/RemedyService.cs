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



        public async Task PostNewIdeaAsync(Idea idea, WordPressUser user, string user3and3)
        {

            var request = new New_Create_Operation_0Request();

            AuthenticationInfo authInfo = new AuthenticationInfo();
            authInfo.userName = ""; // give a valid AR user name 
            authInfo.password = ""; // give a valid password 


                
            try
            {
               var response = await _remedyClient.New_Create_Operation_0Async(authInfo,
                    Customer_Login_ID: user3and3,
                    Summary: idea.Title,
                    Description: idea.Description,
                    Requested_For: Requested_ForType.Individual,
                    Location_Company: "City of Edmonton",
                    Work_Order_Template_Used: "Work Order - Generic - Create",
                    TemplateName: "Work Order - Generic - Create",
                    TemplateId: "IDGC9I1TFF46MANHQ5SWAK91QGC3AB",
                    Customer_First_Name: user.FirstName,
                    Customer_Last_Name: user.LastName,
                    Categorization_Tier_1: "Information",
                    Categorization_Tier_2: "Adhoc");
            }
            catch (Exception err)
            {

                throw;
            }

        }
    }
}
