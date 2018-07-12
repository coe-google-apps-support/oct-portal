using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Issues.Core.Data;
using CoE.Ideas.Shared.Data;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoE.Issues.Core.Services
{
    internal class RemoteIssueRepository : IIssueRepository, IRemoteRepository
    {
        public RemoteIssueRepository(IWordPressUserSecurity wordPressUserSecurity,
            IOptions<RemoteIssueRepositoryOptions> options,
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


        public Task<Issue> AddIssueAsync(Issue Issue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        public async Task<Issue> GetIssueAsync(Guid id)
        {
            return await ExecuteAsync(async client =>
            {
                _logger.Information("Retrieving Issue from {Url}", client.BaseAddress + id.ToString());
                var ideaString = await client.GetStringAsync(id.ToString());
                _logger.Debug("Retrieved initative data: {Text}", ideaString);
                var contractResolver = new IssueContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<Issue>(ideaString, settings);
            });
        }

        public async Task<Issue> GetIssueAsync(int id)
        {
            return await ExecuteAsync(async client =>
            {
                _logger.Information("Retrieving Issue from {Url}", client.BaseAddress + id.ToString());
                var ideaString = await client.GetStringAsync(id.ToString());
                _logger.Debug("Retrieved initative {IssueId} data: {Text}", id, ideaString);
                var contractResolver = new IssueContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<Issue>(ideaString, settings);
            });
        }

        public async Task<PagedResultSet<IssueInfo>> GetIssuesAsync(string filter, int pageNumber, int pageSize)
        {
            return await ExecuteAsync(async client =>
            {
                var queryBuilder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    queryBuilder.Append(queryBuilder.Length == 0 ? "?" : "&");
                    queryBuilder.Append("contains=");
                    queryBuilder.Append(filter);
                }
                if (pageNumber > 0)
                {
                    queryBuilder.Append(queryBuilder.Length == 0 ? "?" : "&");
                    queryBuilder.Append("page=");
                    queryBuilder.Append(pageNumber);
                }
                if (pageSize > 0)
                {
                    queryBuilder.Append(queryBuilder.Length == 0 ? "?" : "&");
                    queryBuilder.Append("pageSize=");
                    queryBuilder.Append(pageSize);
                }

                var response = await client.GetAsync(queryBuilder.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string totalCountString = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
                    int totalCount = 0;
                    if (!string.IsNullOrWhiteSpace(totalCountString))
                        int.TryParse(totalCountString, out totalCount);

                    var ideaString = await response.Content.ReadAsStringAsync();
                    var contractResolver = new IssueContractResolver();
                    var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                    var returnValues = JsonConvert.DeserializeObject<IEnumerable<IssueInfo>>(ideaString, settings).ToList();
                    return PagedResultSet.Create(returnValues, pageNumber, pageSize, totalCount);
                }
                else
                {
                    var ex = new InvalidOperationException($"Response from remote webservice did not indicate success ({response.StatusCode})");
                    ex.Data["HttpResponse"] = response;
                    throw ex;
                }

            });
        }

        
        private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (_user == null)
                _user = await _wordPressUserSecurity.TryCreateAdminServicePrincipalAsync();

            if (_user == null)
                throw new InvalidOperationException("User must be set before making remote calls to Issue service");

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = _baseUri;
                _wordPressUserSecurity.SetWordPressCredentials(client, cookieContainer, _user);
                return await callback(client);
            }
        }



        public Task<Issue> UpdateIssueAsync(Issue issue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        internal class IssueContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }
                return prop;
            }

            
        }

    }
}
