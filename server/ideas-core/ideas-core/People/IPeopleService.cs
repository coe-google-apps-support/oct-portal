using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.People
{
    public interface IPeopleService
    {
        Task<PersonData> GetPersonAsync(string user3and3);
            
    }
}
