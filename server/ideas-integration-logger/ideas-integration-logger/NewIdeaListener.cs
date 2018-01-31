using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Logger
{
    public class NewIdeaListener : IdeaListener
    {
        public NewIdeaListener(IIdeaRepository ideaRepository,
            IWordPressClient wordPressClient,
            IIdeaLogger ideaLogger,
            //IActiveDirectoryUserService activeDirectoryUserService,
            IIdeaServiceBusSender ideaServiceBusSender, 
            ILogger<NewIdeaListener> logger)
        : base(ideaRepository, wordPressClient, logger)
        {
            _ideaLogger = ideaLogger;
            //_activeDirectoryUserService = activeDirectoryUserService;
            _ideaServiceBusSender = ideaServiceBusSender;
            _logger = logger;
        }

        private readonly IIdeaLogger _ideaLogger;
        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly IIdeaServiceBusSender _ideaServiceBusSender;
        private readonly ILogger<NewIdeaListener> _logger;

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaCreated && message.IdeaId > 0;
        }


        protected override async Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser wordPressUser)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");
            if (wordPressUser == null)
                throw new ArgumentNullException("wordPressUser");
            if (string.IsNullOrWhiteSpace(wordPressUser.Email))
                throw new ArgumentOutOfRangeException("wordpressUser email is empty");

            //UserPrincipal adUser;
            //try
            //{
            //    adUser = _activeDirectoryUserService.GetADUser(wordPressUser.Email);
            //}
            //catch (Exception err)
            //{
            //    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }: { err.Message }");
            //}

            //if (adUser == null)
            //    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }");

            Google.Apis.Sheets.v4.Data.AppendValuesResponse loggerResponse;
            try
            {
                loggerResponse = await _ideaLogger.LogIdeaAsync(idea, wordPressUser, adUser: null);
            }
            catch (Exception err)
            {
                Trace.TraceError($"Unable to log idea: { err.Message}");
                loggerResponse = null;
            }

            try
            {
                await _ideaServiceBusSender.SendIdeaMessageAsync(
                    idea, 
                    IdeaMessageType.IdeaLogged, 
                    headers => 
                    {
                        headers["logWasSuccessfull"] = loggerResponse != null;
                        headers["RangeUpdated"] = loggerResponse?.TableRange;
                    });
            }
            catch (Exception err)
            {
                Trace.TraceError($"Unable to send IdeaLogged message: { err.Message }");
            }

        }
    }
}
