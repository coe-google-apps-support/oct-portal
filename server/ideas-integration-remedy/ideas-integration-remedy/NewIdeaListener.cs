using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CoE.Ideas.Remedy
{
    public class NewIdeaListener : IdeaListener
    {
        public NewIdeaListener(IIdeaRepository ideaRepository, 
            IWordPressClient wordPressClient,
            IRemedyService remedyService,
            //IActiveDirectoryUserService activeDirectoryUserService,
            ITopicClient topicClient,
            ILogger<NewIdeaListener> logger,
            Serilog.ILogger seriLogger)
            : base(ideaRepository, wordPressClient, logger)
        {
            _remedyService = remedyService ?? throw new ArgumentNullException("remedyService");
            //_activeDirectoryUserService = activeDirectoryUserService ?? throw new ArgumentNullException("activeDirectoryUserService");
            _topicClient = topicClient ?? throw new ArgumentNullException("topicClient");
            _logger = seriLogger ?? throw new ArgumentNullException("seriLogger");
        }

        private readonly IRemedyService _remedyService;
        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly ITopicClient _topicClient;
        private readonly Serilog.ILogger _logger;

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaCreated;
        }

        protected override async Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser wordPressUser)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");
            //if (wordPressUser == null)
            //    throw new ArgumentNullException("wordPressUser");
            //if (string.IsNullOrWhiteSpace(wordPressUser.Email))
            //    throw new ArgumentOutOfRangeException("wordpressUser email is empty");

            using (LogContext.PushProperty("InitiativeId", idea.Id))
            {
                _logger.Information("Begin ProcessIdeaMessage");
                Stopwatch rootWatch = new Stopwatch(), watch = new Stopwatch();
                rootWatch.Start();
                watch.Start();

                UserPrincipal adUser = null;
                try
                {
                //    adUser = _activeDirectoryUserService.GetADUser(wordPressUser.Email);
                }
                //catch (Exception err)
                //{
                //    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }: { err.Message }");
                //}

                //if (adUser == null)
                //    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }");
                finally
                {
                    watch.Restart();
                }

                string remedyTicketId = null;
                try
                {
                    remedyTicketId = await _remedyService.PostNewIdeaAsync(idea, wordPressUser, adUser?.SamAccountName);
                    _logger.Information("Created Remedy Work Order in {ElapsedMilliseconds}", watch.ElapsedMilliseconds);
                }
                finally
                {
                    watch.Restart();
                }

                try
                {
                    var returnMessage = new Message
                    {
                        Label = "Remedy Work Item Created"
                    };
                    returnMessage.UserProperties["IdeaId"] = idea.Id;
                    returnMessage.UserProperties["WorkItemId"] = remedyTicketId;
                    await _topicClient.SendAsync(returnMessage);
                    _logger.Information("Send remedy work order created message to service bus in {ElapsedMilliseconds}", watch.ElapsedMilliseconds);
                }
                finally
                {
                    // technically we don't need to do this (https://stackoverflow.com/questions/24140261/should-i-stop-stopwatch-at-the-end-of-the-method)
                    // but I think it looks clean
                    watch.Stop();
                    watch = null;
                }

                _logger.Information("Processed message in {ElapsedMilliseconds}", rootWatch.ElapsedMilliseconds);
            }
        }
    }
}
