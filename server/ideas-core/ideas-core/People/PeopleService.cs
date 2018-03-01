using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.People
{
    internal class PeopleService : IPeopleService
    {
        public PeopleService(IOptions<PeopleServiceOptions> options)
        {
            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");

            _serviceUrl = options.Value.ServiceUrl;
            if (_serviceUrl == null)
                throw new ArgumentOutOfRangeException("PeopleServiceOptions ServiceUrl must be specified");

            // ensure _serviceUrl ends with "/", because we'll be appending 
            if (!_serviceUrl.ToString().EndsWith("/"))
                _serviceUrl = new Uri(_serviceUrl + "/");
        }

        private readonly Uri _serviceUrl;

        public async Task<PersonData> GetPersonAsync(string user3and3)
        {
            if (string.IsNullOrWhiteSpace(user3and3))
                throw new ArgumentNullException("user3and3");

            var client = GetHttpClient();

            string userDataString;
            try
            {
                userDataString = await client.GetStringAsync($"OrganizationUnits/NetworkId/{user3and3}");
            }
            catch (Exception err)
            {
                throw new InvalidOperationException($"Unable to get data for user {user3and3}: {err.Message}", err);
            }

            if (string.IsNullOrWhiteSpace(userDataString))
                throw new InvalidOperationException($"Unable to get data for user {user3and3}");

            // there's lots of information we can get from the userData, but right now all we care about is the email
            dynamic userData = Newtonsoft.Json.Linq.JObject.Parse(userDataString);
            return new PersonData() { Email = userData.Mail, DisplayName = $"{ userData.GivenName } { userData.Surname }"};
        }

        protected virtual HttpClient GetHttpClient()
        {

            var client = new HttpClient
            {
                BaseAddress = _serviceUrl
            };

            // easy - no credentials to set :)
            return client;
        }

    }
}
