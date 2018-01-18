using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy;
using COE_WOI_WorkOrderInterface_WS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ideas_integration_remedy_tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestCreateRemedyWorkOrder()
        { 
            var client = new New_Port_0PortTypeClient(
                New_Port_0PortTypeClient.EndpointConfiguration.New_Port_0Soap,
                new EndpointAddress(""));
            IRemedyService svc = new RemedyService(client);

            // mock Idea
            var newIdea = new Idea()
            {
                Title = "Test Idea 1",
                Description = "Test Idea 1 Contents"
            };

            // mock user 
            var newUser = new WordPressUser()
            {
                FirstName = "", LastName = ""
            };


            await svc.PostNewIdeaAsync(newIdea, newUser, "COE\\fakeuser");
        }
    }
}
