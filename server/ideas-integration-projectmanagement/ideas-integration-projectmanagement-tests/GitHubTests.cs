using CoE.Ideas.ProjectManagement.Core;
using CoE.Ideas.ProjectManagement.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace CoE.Ideas.ProjectManagement.Tests
{
    [TestClass]
    public class GitHubTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            serviceProvider = new TestConfiguration(config)
                .ConfigureBasicServices()
                .ConfigureProjectManagementServices()
                .BuildServiceProvider();
        }
        private static ServiceProvider serviceProvider;


        [TestMethod]
        public async Task MockGitHubIssueEvent()
        {
            var gitHubController = new GitHubIssuesController(
                serviceProvider.GetRequiredService<IExtendedProjectManagementRepository>());

            var sampleGitHubIssueEvent = SampleData.GetSampleGitHubIssueEvent();

            await gitHubController.Post(sampleGitHubIssueEvent);

        }
    }
}
