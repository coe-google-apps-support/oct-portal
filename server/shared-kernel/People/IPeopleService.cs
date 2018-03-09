using System.Threading.Tasks;

namespace CoE.Ideas.Shared.People
{
    public interface IPeopleService
    {
        Task<PersonData> GetPersonAsync(string user3and3);
    }
}
