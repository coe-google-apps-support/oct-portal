using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    public interface IMailmanEnabledSheetReader
    {
        Task<IDictionary<string, object>> GetValuesAsync(dynamic mergeTemplate, long ideaId);
        Task<IDictionary<string, object>> GetValuesAsync(dynamic mergeTemplate, string range);

        Task<dynamic> GetMergeTemplateAsync(string templateName);
    }
}