using System.Collections.Generic;
using System.Threading.Tasks;
using CoE.Ideas.Integration.Notification;

namespace CoE.Ideas.EndToEnd.Tests.Mocks
{
    internal class MockMailmanEnabledSheetReader : IMailmanEnabledSheetReader
    {
        public Task<dynamic> GetMergeTemplateAsync(string templateName)
        {
            return Task.FromResult(new object());
        }

        public Task<IDictionary<string, object>> GetValuesAsync(dynamic mergeTemplate, long ideaId)
        {
            IDictionary<string, object> returnValue = new Dictionary<string, object>
            {
                ["InitiativeId"] = ideaId
            };
            return Task.FromResult(returnValue);
        }

        public Task<IDictionary<string, object>> GetValuesAsync(dynamic mergeTemplate, string range)
        {
            IDictionary<string, object> returnValue = new Dictionary<string, object>();
            return Task.FromResult(returnValue);
        }
    }
}