using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
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


        protected virtual Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser wordPressUser) { return Task.CompletedTask; }

        protected virtual bool ShouldProcessMessage(IdeaMessage message)
        {
            return true;
        }

        public virtual async Task<MessageProcessResponse> OnMessageRecevied(IdeaMessage message, IDictionary<string, object> properties)
        {

            try
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                if (ShouldProcessMessage(message))
                {

                    // Get the actual idea and the user who created it
                    var ideaTask = _ideaRepository.GetIdeaAsync(message.IdeaId);
                    _wordPressClient.JwtCredentials = properties["AuthToken"]?.ToString();
                    var userTask = _wordPressClient.GetCurrentUserAsync();

                    var idea = await ideaTask;
                    var user = await userTask;

                    await ProcessIdeaMessage(message, idea, user);
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
