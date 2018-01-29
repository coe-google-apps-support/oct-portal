using CoE.Ideas.Core;
using CoE.Ideas.Remedy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    public class MockServiceBus
    {
        public MockServiceBus(NewIdeaListener remedyIntegrationListener)
        {
            _remedyIntegrationListener = remedyIntegrationListener ?? throw new ArgumentNullException("remedyIntegrationListener");
        }

        private NewIdeaListener _remedyIntegrationListener;

        public async Task OnIdeaAdded(Idea idea)
        {
            await _remedyIntegrationListener.OnMessageRecevied(
                new Core.ServiceBus.IdeaMessage()
                {
                    IdeaId = idea.Id,
                    Type = Core.ServiceBus.IdeaMessageType.IdeaCreated
                }, new Dictionary<string, object>()
                {
                    { "AuthToken", "FakeToken" },
                    { "IdeaMessageType", Core.ServiceBus.IdeaMessageType.IdeaCreated.ToString() }
                });
        }
    }
}
