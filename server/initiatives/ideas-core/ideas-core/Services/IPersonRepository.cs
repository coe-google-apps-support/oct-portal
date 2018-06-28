using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    public interface IPersonRepository
    {
        Task<int?> GetPersonIdByEmailAsync(string email);
        Task<Person> GetPersonAsync(int id);

        Task<Person> CreatePerson(string firstName, string lastName, string email, string phoneNumber,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
