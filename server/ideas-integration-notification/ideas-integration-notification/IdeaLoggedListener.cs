using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    public class IdeaLoggedListener : IdeaListener
    {
        public IdeaLoggedListener(IIdeaRepository ideaRepository,
            IWordPressClient wordPressClient, 
            IMailmanEnabledSheetReader mailmanSheetReader,
            string mergeTemplateName)
        : base(ideaRepository, wordPressClient)
        {
            _mailmanSheetReader = mailmanSheetReader;
            _mergeTemplateName = mergeTemplateName;
        }

        private readonly IMailmanEnabledSheetReader _mailmanSheetReader;
        private readonly string _mergeTemplateName;

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaLogged;
        }


        public override async Task<MessageProcessResponse> OnMessageRecevied(IdeaMessage message, IDictionary<string, object> properties)
        {
            try
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                if (ShouldProcessMessage(message))
                {
                    var mergeTemplate = await _mailmanSheetReader.GetMergeTemplateAsync(_mergeTemplateName);

                    var ideaData = await _mailmanSheetReader.GetValuesAsync(message.IdeaId);

                    if (mergeTemplate != null && ideaData != null)
                    {
                        //TODO: Send Email
                        throw new NotImplementedException();
                    }

                }

                return MessageProcessResponse.Complete;
            }
            catch (Exception err)
            {
                // log the error
                System.Diagnostics.Trace.TraceError($"Error processing idea message: { err.Message }");

                // abandon message?
                return MessageProcessResponse.Abandon;
            }
        }

        protected override Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser wordPressUser)
        {
            // TODO: Refactor this as it is never called
            throw new NotImplementedException();
        }
    }
}
