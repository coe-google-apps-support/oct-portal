using System.Collections.Generic;
using System.Threading.Tasks;
using CoE.Ideas.Integration.Notification;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockEmailService : IEmailService
    {
        public MockEmailService()
        {
            emailsSent = new List<SentEmail>();
        }

        private readonly ICollection<SentEmail> emailsSent;
        public IEnumerable<SentEmail> EmailsSent { get { return emailsSent; } }

        public Task SendEmailAsync(dynamic mergeTemplate, IDictionary<string, object> ideaData)
        {
            emailsSent.Add(new SentEmail() { MergeTemplate = mergeTemplate, InitiativeData = ideaData });
            return Task.CompletedTask;
        }

        internal class SentEmail
        {
            public dynamic MergeTemplate { get; set; }
            public IDictionary<string, object> InitiativeData { get; set; }
        }
    
    }
}