using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    public class IdeaLoggedListener : IdeaListener
    {
        public IdeaLoggedListener(IIdeaRepository ideaRepository,
            IWordPressClient wordPressClient, 
            IMailmanEnabledSheetReader mailmanSheetReader,
            IEmailService emailService,
            ILogger<IdeaLoggedListener> logger,
            Serilog.ILogger seriLogger,
            string mergeTemplateName)
        : base(ideaRepository, wordPressClient, logger)
        {
            _mailmanSheetReader = mailmanSheetReader;
            _emailService = emailService;
            _mergeTemplateName = mergeTemplateName;
            _logger = seriLogger;
        }

        private readonly IMailmanEnabledSheetReader _mailmanSheetReader;
        private readonly IEmailService _emailService;
        private readonly Serilog.ILogger _logger;
        private readonly string _mergeTemplateName;

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaLogged;
        }


        public override async Task<MessageProcessResponse> OnMessageRecevied(IdeaMessage message, IDictionary<string, object> properties)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            using (LogContext.PushProperty("InitiativeId", message.IdeaId))
            {

                try
                {
                    if (ShouldProcessMessage(message))
                    {
                        await ProcessIdeaLoggedMessage(message, properties);
                    }

                    return MessageProcessResponse.Complete;
                }
                catch (Exception err)
                {
                    // log the error
                    System.Diagnostics.Trace.TraceError($"Error processing idea message: { err.Message }");
                    _logger.Error("Error processing idea message: {ErrorMessage}", err.Message);
    
                    // abandon message?
                    return MessageProcessResponse.Abandon;
                }
            }
        }

        protected virtual async Task ProcessIdeaLoggedMessage(IdeaMessage message, IDictionary<string, object> properties)
        {
            _logger.Information("Recieved message that an initiative has been logged");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var mergeTemplate = await _mailmanSheetReader.GetMergeTemplateAsync(_mergeTemplateName);
            if (mergeTemplate == null)
            {
                // intentionally not putting "MergeTemplateName" as a property of Serilog
                throw new InvalidOperationException($"Unable to get merge template with name { _mergeTemplateName }");
            }
            else
            {
                string ideaRange = properties.ContainsKey("RangeUpdated") ? properties["RangeUpdated"] as string : null;

                IDictionary<string, object> ideaData;
                if (string.IsNullOrWhiteSpace(ideaRange))
                {
                    ideaData = await _mailmanSheetReader.GetValuesAsync(mergeTemplate, message.IdeaId);
                }
                else
                {
                    ideaData = await _mailmanSheetReader.GetValuesAsync(mergeTemplate, ideaRange);
                }

                if (ideaData == null)
                {
                    _logger.Error("Unable to get initiative data for initiative id {InitiativeId} and range: " + ideaRange);
                }
                else
                {
                    _emailService.SendEmailAsync(mergeTemplate, ideaData);
                    _logger.Information("Email Notification sent. Time Elapsed: {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
                }
            }
        }

        //protected override Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser wordPressUser)
        //{
        //    // TODO: Refactor this as it is never called
        //    throw new NotImplementedException();
        //}
    }
}
