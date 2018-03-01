using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    public class IdeaLoggedListener
    {
        public IdeaLoggedListener(IMailmanEnabledSheetReader mailmanSheetReader,
            IEmailService emailService,
            IInitiativeMessageReceiver initiativeMessageReceiver,
            Serilog.ILogger logger,
            string mergeTemplateName)
        {
            _mailmanSheetReader = mailmanSheetReader ?? throw new ArgumentNullException("mailmanSheetReader");
            _emailService = emailService ?? throw new ArgumentNullException("emailService");
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _mergeTemplateName = mergeTemplateName;

            initiativeMessageReceiver.ReceiveMessages(initiativeLoggedHandler: OnInitiativeLogged);
        }

        private readonly IMailmanEnabledSheetReader _mailmanSheetReader;
        private readonly IEmailService _emailService;
        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly Serilog.ILogger _logger;
        private readonly string _mergeTemplateName;


        protected virtual async Task OnInitiativeLogged(InitiativeLoggedEventArgs args, CancellationToken token)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");

            using (LogContext.PushProperty("InitiativeId", args.Initiative.Id))
            {
                _logger.Information("Recieved message that an initiative has been logged");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var mergeTemplate = await GetMessageTemplate();
                if (mergeTemplate != null)
                {
                    string ideaRange = args.RangeUpdated;

                    IDictionary<string, object> ideaData = await GetInitiativeData(ideaRange, mergeTemplate, args.Initiative.Id);

                    if (ideaData != null)
                    { 
                        await _emailService.SendEmailAsync(mergeTemplate, ideaData);
                        _logger.Information("Email Notification sent. Time Elapsed: {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
                    }
                }
            }

        }

        protected virtual async Task<dynamic> GetMessageTemplate()
        {
            dynamic mergeTemplate;
            try
            {
                mergeTemplate = await _mailmanSheetReader.GetMergeTemplateAsync(_mergeTemplateName);
            }
            catch (Exception err)
            {
                _logger.Error(err, "Unable to get merge template with name {MergeTemplate}: {ErrorMessage}", _mergeTemplateName, err.Message);
                throw new InvalidOperationException($"Unable to get merge template with name {_mergeTemplateName}: {err.Message}", err);
            }
            if (mergeTemplate == null)
            {
                // intentionally not putting "MergeTemplateName" as a property of Serilog
                _logger.Error("Unable to get merge template with name {MergeTemplate}", _mergeTemplateName);
                throw new InvalidOperationException($"Unable to get merge template with name { _mergeTemplateName }");
            }
            return mergeTemplate;
        }

        protected virtual async Task<IDictionary<string, object>> GetInitiativeData(string ideaRange, dynamic mergeTemplate, long initiativeId)
        {
            IDictionary<string, object> ideaData;
            if (string.IsNullOrWhiteSpace(ideaRange))
            {
                ideaData = await _mailmanSheetReader.GetValuesAsync(mergeTemplate, initiativeId);
            }
            else
            {
                ideaData = await _mailmanSheetReader.GetValuesAsync(mergeTemplate, ideaRange);
            }

            if (ideaData == null)
            {
                _logger.Error("Unable to get initiative data for initiative id {InitiativeId} and range: " + ideaRange);
            }

            return ideaData;
        }
    }
}
