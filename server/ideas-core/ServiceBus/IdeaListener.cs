using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public abstract class IdeaListener : IIdeaListener
    {
        public IdeaListener(IIdeaRepository ideaRepository, 
            IWordPressClient wordPressClient,
            ILogger<IdeaListener> logger)
        {
            _ideaRepository = ideaRepository ?? throw new ArgumentNullException("ideaRepository");
            _wordPressClient = wordPressClient ?? throw new ArgumentException("wordPressClient");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }
        private readonly IIdeaRepository _ideaRepository;
        private readonly IWordPressClient _wordPressClient;
        private readonly ILogger<IdeaListener> _logger;


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

                if (ShouldProcessMessage(message) && message.IdeaId > 0)
                {
                    _logger.LogInformation($"Received New Idea Message for idea with id { message.IdeaId }");

                    // Get the actual idea and the user who created it
                    var ideaTask = _ideaRepository.GetIdeaAsync(message.IdeaId);
                    string authToken = properties.ContainsKey("AuthToken") ? properties["AuthToken"]?.ToString() : null;
                    WordPressUser user;
                    if (string.IsNullOrWhiteSpace(authToken))
                    {
                        _logger.LogError("Received New Idea message without an AuthToken");
                        user = null;
                    }
                    else
                    {
                        _wordPressClient.JwtCredentials = authToken;
                        user = await _wordPressClient.GetCurrentUserAsync();
                    }

                    var idea = await ideaTask;

                    await ProcessIdeaMessage(message, idea, user);
                }

                return MessageProcessResponse.Complete;
            }
            catch (Exception err)
            {
                _logger.LogError($"Error processing idea message: { err.Message } - { err.StackTrace }");

                // log the error
                System.Diagnostics.Trace.TraceError($"Error processing idea message: { err.ToString() }");

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
