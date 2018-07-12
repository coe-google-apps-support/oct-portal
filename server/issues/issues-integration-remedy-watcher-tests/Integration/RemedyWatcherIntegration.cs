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
using AutoMapper;
using CoE.Ideas.Shared.People;

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

            mockRemedyService = new Mock<IRemedyService>();
            mockRemedyService.Setup(remedyService =>
                    remedyService.GetRemedyChangedWorkItems(It.IsAny<DateTime>())
                ).Returns(returnValue);

            mockIssueMessageSender = new Mock<IIssueMessageSender>();

            mockRemedyChangedReceiver = new Mock<IRemedyChangedReceiver>();
            mockPeopleService = new Mock<IPeopleService>();

            mockLogger = new Mock<Serilog.ILogger>();

            RemedyCheckerOptions options = new RemedyCheckerOptions()
            {
                ServiceUserName = "user",
                ServicePassword = "password",
                TemplateName = "",
                ApiUrl = "localhost/api",
                TempDirectory = "./Poll Files"
            };

            mockOptions = new Mock<IOptions<RemedyCheckerOptions>>();
            mockOptions.Setup(opt => opt.Value).Returns(options);

            Mapper.Initialize(cfg =>
            {
                //cfg.AddProfile<RemedyIssueMappingProfile>();
            });
            mapper = Mapper.Instance;
        }

        private Mock<IRemedyService> mockRemedyService;
        private Mock<IIssueMessageSender> mockIssueMessageSender;
        private Mock<IRemedyChangedReceiver> mockRemedyChangedReceiver;
        private Mock<IPeopleService> mockPeopleService;
        private Mock<Serilog.ILogger> mockLogger;
        private Mock<IOptions<RemedyCheckerOptions>> mockOptions;

        private IMapper mapper;

        [Test]
        public void PollMocked()
        {
            IRemedyChecker checker = new RemedyChecker(
                mockIssueMessageSender.Object, 
                mockRemedyChangedReceiver.Object,
                mockPeopleService.Object,
                mapper,
                mockLogger.Object, 
                mockOptions.Object);
            checker.Poll();
        }

    }
}