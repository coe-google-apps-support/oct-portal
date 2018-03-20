﻿using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Tests;
using CoE.Ideas.EndToEnd.Tests.Mocks;
using CoE.Ideas.Integration.Notification;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.SbListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoE.Ideas.EndToEnd.Tests
{
    public class TestConfiguration : BaseTestConfiguration
    {
        public TestConfiguration(IConfigurationRoot configuration) : base(configuration)
        {
        }

        public TestConfiguration(IConfigurationRoot configuration,
            IServiceCollection services) : base(configuration, services)
        {
        }


        public ServiceProvider ServiceProvider { get; private set; }

        public new TestConfiguration ConfigureBasicServices()
        {
            base.ConfigureBasicServices();
            return this;
        }

        public TestConfiguration ConfigureIdeaServices()
        {
            //Services.AddScoped<IWordPressClient, MockWordPressClient>();
            //Services.AddScoped<IIdeaRepository, MockIdeaRepository>();
            //Services.AddScoped<IUpdatableIdeaRepository, MockIdeaRepository>();
            base.ConfigureIdeasController();
            return this;
        }

        public TestConfiguration ConfigureIdeaMessaging()
        {

            Services.AddSingleton<SynchronousInitiativeMessageReceiver>();
            Services.AddSingleton<IInitiativeMessageReceiver>(x =>
            {
                return x.GetRequiredService<SynchronousInitiativeMessageReceiver>();
            });
            Services.AddSingleton<IInitiativeMessageSender, SynchronousInitiativeMessageSender>();

            return this;
        }

        public TestConfiguration ConfigureRemedyServices()
        {
            Services.AddSingleton<MockRemedyService>();
            Services.AddSingleton<IRemedyService>(x => x.GetRequiredService<MockRemedyService>());

            // configure the listener that listens for new initiatives and create work order in remedy
            Services.AddSingleton<NewIdeaListener>();

            Services.AddSingleton<RemedyItemUpdatedIdeaListener>();

            return this;
        }

        public TestConfiguration ConfigureNotificationServices()
        {
            // logger first
            Services.AddSingleton<MockIdeaLogger>();
            Services.AddSingleton<Integration.Logger.IIdeaLogger>(x => x.GetRequiredService<MockIdeaLogger>());
            Services.AddSingleton<Integration.Logger.NewIdeaListener>();

            // now notifications
            Services.AddSingleton<MockMailmanEnabledSheetReader>();
            Services.AddSingleton<IMailmanEnabledSheetReader>(x => x.GetRequiredService<MockMailmanEnabledSheetReader>());
            Services.AddSingleton<MockEmailService>();
            Services.AddSingleton<IEmailService>(x => x.GetRequiredService<MockEmailService>());
            Services.AddSingleton(x =>
            {
                return new IdeaLoggedListener(
                    x.GetRequiredService<IMailmanEnabledSheetReader>(),
                    x.GetRequiredService<IEmailService>(),
                    x.GetRequiredService<IInitiativeMessageReceiver>(),
                    x.GetRequiredService<Serilog.ILogger>(),
                    "TestMergeTemplate");
            });


            return this;
        }
    }
}
