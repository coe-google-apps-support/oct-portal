using CoE.Ideas.Core.Data;
using CoE.Ideas.Shared.People;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy
{
    public interface IRemedyService
    {
        Task<string> PostNewIdeaAsync(Initiative idea, PersonData personData);
    }
}
