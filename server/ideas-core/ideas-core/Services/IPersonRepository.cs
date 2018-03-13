using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    public interface IPersonRepository
    {
        Task<int> GetPersonIdByEmailAsync(string email);
        Task<Person> GetPersonAsync(int id);
    }
}
