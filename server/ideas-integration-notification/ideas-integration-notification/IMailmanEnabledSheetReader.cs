using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    public interface IMailmanEnabledSheetReader
    {
        Task<IDictionary<string, object>> GetValuesAsync(long ideaId);

        Task<dynamic> GetMergeTemplateAsync(string templateName);
    }
}