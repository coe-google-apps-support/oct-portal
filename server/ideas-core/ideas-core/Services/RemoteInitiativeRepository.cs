using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoE.Ideas.Core.Services
{
    internal class RemoteInitiativeRepository : IInitiativeRepository
    {
        public RemoteInitiativeRepository(IWordPressUserSecurity wordPressUserSecurity,
            IOptions<RemoteInitiativeRepositoryOptions> options,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(wordPressUserSecurity);
            EnsureArg.IsNotNull(options);
            EnsureArg.IsNotNull(logger);

            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            if (string.IsNullOrWhiteSpace(options.Value.IdeasApiUrl))
                throw new ArgumentNullException("WordPressUrl");

            _logger = logger;
            _wordPressUserSecurity = wordPressUserSecurity;

            if (options.Value.IdeasApiUrl.EndsWith("/"))
                _baseUri = new Uri(options.Value.IdeasApiUrl);
            else
                _baseUri = new Uri(options.Value.IdeasApiUrl + "/");

        }

        public void SetUser(ClaimsPrincipal user)
        {
            _user = user;
        }

        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly Uri _baseUri;
        private readonly Serilog.ILogger _logger;

        private ClaimsPrincipal _user;


        public Task<Initiative> AddInitiativeAsync(Initiative initiative)
        {
            throw new NotSupportedException();
        }

        public async Task<Initiative> GetInitiativeAsync(Guid id)
        {
            return await ExecuteAsync(async client =>
            {
                _logger.Information("Retrieving initiative from {Url}", client.BaseAddress + id.ToString());
                var ideaString = await client.GetStringAsync(id.ToString());
                _logger.Debug("Retrieved initative data: {Text}", ideaString);
                return JsonConvert.DeserializeObject<Initiative>(ideaString);
            });
        }

        public async Task<Initiative> GetInitiativeAsync(int id)
        {
            return await ExecuteAsync(async client =>
            {
                _logger.Information("Retrieving initiative from {Url}", client.BaseAddress + id.ToString());
                var ideaString = await client.GetStringAsync(id.ToString());
                _logger.Debug("Retrieved initative {InitiativeId} data: {Text}", id, ideaString);
                return JsonConvert.DeserializeObject<Initiative>(ideaString);
            });
        }

        public async Task<IEnumerable<InitiativeInfo>> GetInitiativesAsync()
        {
            return await ExecuteAsync(async client =>
            {
                var ideaString = await client.GetStringAsync(string.Empty);
                return JsonConvert.DeserializeObject<IEnumerable<InitiativeInfo>>(ideaString);
            });
        }

        public async Task<IEnumerable<InitiativeInfo>> GetInitiativesByStakeholderPersonIdAsync(int personId)
        {
            return await ExecuteAsync(async client =>
            {
                var ideaString = await client.GetStringAsync("?View=Mine");
                return JsonConvert.DeserializeObject<IEnumerable<InitiativeInfo>>(ideaString);
            });
        }

        public async Task<IEnumerable<InitiativeStep>> GetInitiativeStepsAsync(int initiativeId)
        {
            return await ExecuteAsync(async client =>
            {
                var ideaString = await client.GetStringAsync(initiativeId.ToString() + "/steps");
                return JsonConvert.DeserializeObject<IEnumerable<InitiativeStep>>(ideaString);
            });
        }

        public Task<Initiative> UpdateInitiativeAsync(Initiative initiative)
        {
            throw new NotSupportedException();
        }

        public async Task<Initiative> GetInitiativeByWorkOrderIdAsync(string workOrderId)
        {
            throw new NotImplementedException();
            //return await ExecuteAsync(async client =>
            //{
            //    var ideaString = await client.GetStringAsync(id.ToString());
            //    return JsonConvert.DeserializeObject<Initiative>(ideaString);
            //});
        }

        private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (_user == null)
                throw new InvalidOperationException("User must be set before making remote calls to Initiative service");

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = _baseUri;
                _wordPressUserSecurity.SetWordPressCredentials(client, cookieContainer, _user);
                return await callback(client);
            }
        }
    }
}
