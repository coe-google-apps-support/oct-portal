using CoE.Ideas.Core;
using CoE.Ideas.Remedy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ideas_integration_remedy_tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestCreateRemedyWorkOrder()
        {
            IRemedyService svc = RemedyServiceFactory.CreateRemedyService();

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
