using CoE.Ideas.Core;
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
                new BasicHttpBinding(BasicHttpSecurityMode.None),
                new EndpointAddress(""));
            IRemedyService svc = new RemedyService(client);

            // mock Idea
            var newIdea = new Idea()
            {
                Title = "Test Idea 1",
                Description = "Test Idea 1 Contents"
            };
            await svc.PostNewIdeaAsync(newIdea, "COE\\fakeuser");
        }
    }
}
