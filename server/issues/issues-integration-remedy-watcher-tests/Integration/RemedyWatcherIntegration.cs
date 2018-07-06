using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using System.Collections.Generic;
using Moq;
using CoE.Issues.Core.ServiceBus;
using Microsoft.Extensions.Options;

namespace CoE.Issues.Remedy.Watcher.Tests
{
    public class RemedyWatcherIntegration
    {
        [SetUp]
        public void Setup()
        {
            IEnumerable<OutputMapping1GetListValues> returnValue = new OutputMapping1GetListValues[1] 
            {
                new OutputMapping1GetListValues()
                {
                    Description = "This is a real long description."
                }
            };

            Mock<IRemedyService> mockRemedyService = new Mock<IRemedyService>();
            mockRemedyService.Setup(remedyService =>
                    remedyService.GetRemedyChangedWorkItems(It.IsAny<DateTime>())
                ).Returns(Task.FromResult(returnValue));
            remedyService = mockRemedyService.Object;

            Mock<IIssueMessageSender> issueMock = new Mock<IIssueMessageSender>();
            issueMessageSender = issueMock.Object;

            Mock<Serilog.ILogger> loggerMock = new Mock<Serilog.ILogger>();
            logger = loggerMock.Object;

            RemedyCheckerOptions options = new RemedyCheckerOptions()
            {
                ServiceUserName = "user",
                ServicePassword = "password",
                TemplateName = "",
                ApiUrl = "localhost/api",
                TempDirectory = "./Poll Files"
            };

            Mock<IOptions<RemedyCheckerOptions>> optionsMock = new Mock<IOptions<RemedyCheckerOptions>>();
            optionsMock.Setup(opt => opt.Value).Returns(options);
            remedyOptions = optionsMock.Object;
        }

        private IRemedyService remedyService;
        private IIssueMessageSender issueMessageSender;
        private Serilog.ILogger logger;
        private IOptions<RemedyCheckerOptions> remedyOptions;

        [Test]
        public async Task GetChanges()
        {
            IRemedyChecker checker = new RemedyChecker(remedyService, issueMessageSender, logger, remedyOptions);
            await checker.Poll();
        }
    }
}