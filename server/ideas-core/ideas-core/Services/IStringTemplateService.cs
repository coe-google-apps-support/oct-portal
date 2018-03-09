using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.WordPress;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal interface IStringTemplateService
    {
        Task<string> GetStatusChangeTextAsync(InitiativeStatus status, WordPressUser assignee, bool isPastTense = false);
    }
}
