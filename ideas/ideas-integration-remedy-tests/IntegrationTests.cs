using CoE.Ideas.Core;
using CoE.Ideas.Remedy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ideas_integration_remedy_tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCreateRemedyWorkOrder()
        {
            IRemedyService svc = new RemedyService();

            // mock Idea
            var newIdea = new Idea()
            {
                Title = "Test Idea 1",
                Description = "Test Idea 1 Contents"
            };
            svc.PostNewIdea(newIdea, "COE\\fakeuser");
        }
    }
}
