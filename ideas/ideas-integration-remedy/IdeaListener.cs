using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy
{
    public abstract class IdeaListener : IIdeaListener
    {
        public IdeaListener(IIdeaRepository ideaRepository, IWordPressClient wordPressClient)
        {
            _ideaRepository = ideaRepository ?? throw new ArgumentNullException("ideaRepository");
            _wordPressClient = wordPressClient ?? throw new ArgumentException("wordPressClient");
        }
        private readonly IIdeaRepository _ideaRepository;
        private readonly IWordPressClient _wordPressClient;


        protected abstract Task ProcessIdeaMessage(IdeaMessage message, Idea idea, string user3and3);

        protected virtual bool ShouldProcessMessage(IdeaMessage message)
        {
            return true;
        }

        public async Task<MessageProcessResponse> OnMessageRecevied(IdeaMessage message)
        {

            try
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                if (ShouldProcessMessage(message))
                {

                    // Get the actual idea and the user who created it
                    var ideaTask = _ideaRepository.GetIdeaAsync(message.IdeaId);
                    var userTask = _wordPressClient.GetCurrentUserAsync();

                    var idea = await ideaTask;
                    var user = await userTask;

                    //TODO: get user's 3+3
                    
                    string user3and3 = null;

                    await ProcessIdeaMessage(message, idea, user3and3);
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

        public void OnError(Exception err)
        {
            // TODO: log error and continue
        }

        public void OnWait()
        {
            // what to do?
            // we could log that we're waiting, but maybe we'll just do nothing
        }
    }
}
