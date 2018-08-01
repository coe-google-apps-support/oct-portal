using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Issues.Core.Data;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;


namespace CoE.Issues.Core.Services
{
    internal class PersonRepository : IPersonRepository
    {
        public PersonRepository(IWordPressRepository wordPressRepository)
        {
            EnsureArg.IsNotNull(wordPressRepository);
            _wordPressRepository = wordPressRepository;
        }

        private readonly IWordPressRepository _wordPressRepository;

        public async Task<Person> GetPersonAsync(int id)
        {
            var user = await _wordPressRepository.GetUserAsync(id);
            if (user == null)
                return null;
            else
                return Person.Create(id: user.Id, name: user.Name, email: user.Email);
        }

        public async Task<int?> GetPersonIdByEmailAsync(string email)
        {
            var user = await _wordPressRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null; // should we throw an exception?
            else
                return user.Id;
        }


        public async Task<Person> CreatePerson(string firstName, string lastName, string email, string phoneNumber,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            var user = await _wordPressRepository.CreateUser(firstName, lastName, email, phoneNumber, cancellationToken);
            return Person.Create(id: user.Id, name: user.Name, email: user.Email);
        }
    }
}
