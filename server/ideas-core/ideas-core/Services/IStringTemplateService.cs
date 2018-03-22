using CoE.Ideas.Core.Data;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal interface IStringTemplateService
    {
        Task<string> GetStatusChangeTextAsync(InitiativeStatus status, bool isPastTense = false);
    }
}
