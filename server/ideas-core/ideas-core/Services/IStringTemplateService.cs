using CoE.Ideas.Core.Data;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal interface IStringTemplateService
    {
        Task<string> GetStatusChangeTextAsync(InitiativeStatus status, Person assignee, bool isPastTense = false);
    }
}
