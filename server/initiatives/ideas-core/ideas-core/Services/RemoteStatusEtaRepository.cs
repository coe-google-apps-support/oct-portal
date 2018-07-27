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
    internal class RemoteStatusEtaRepository : IInitiativeStatusEtaRepository, IRemoteRepository
    {
        public RemoteStatusEtaRepository(IWordPressUserSecurity wordPressUserSecurity,
            IOptions<RemoteStatusEtaRepositoryOptions> options,
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

        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly Uri _baseUri;
        private readonly Serilog.ILogger _logger;

        private ClaimsPrincipal _user;

        public async Task<IEnumerable<StatusEta>> GetStatusEtasAsync()
        {
            return await ExecuteAsync(async client =>
            {
                var valuesString = await client.GetStringAsync("statusEtas");
                var contractResolver = new RemoteInitiativeRepository.InitiativeContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<IEnumerable<StatusEta>>(valuesString, settings);
            });
        }

        public void SetUser(ClaimsPrincipal user)
        {
            _user = user;
        }

        private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (_user == null)
                _user = await _wordPressUserSecurity.TryCreateAdminServicePrincipalAsync();

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
