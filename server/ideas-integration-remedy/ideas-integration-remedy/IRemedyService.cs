using CoE.Ideas.Core.Data;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy
{
    public interface IRemedyService
    {
        Task<string> PostNewIdeaAsync(Initiative idea, string user3and3);
    }
}
