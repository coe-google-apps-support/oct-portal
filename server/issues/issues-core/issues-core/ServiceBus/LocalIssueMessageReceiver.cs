
using System;
using System.Security.Claims;
using CoE.Issues.Core.Services;
using EnsureThat;
using Microsoft.Azure.ServiceBus;

namespace CoE.Issues.Core.ServiceBus
{
    internal class LocalIssueMessageReceiver : IssueMessageReceiver
    {
        public LocalIssueMessageReceiver(IMessageReceiver messageReceiver,
                Serilog.ILogger logger,
                IServiceProvider serviceProvider
                ) : base(messageReceiver, logger)
        {
            EnsureArg.IsNotNull(serviceProvider);
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;

    }
}
