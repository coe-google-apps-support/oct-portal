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
using CoE.Ideas.Core.Data;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoE.Ideas.Core.Services
{
    internal class RemoteInitiativeRepository : IInitiativeRepository, IRemoteRepository
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


        public Task<Initiative> AddInitiativeAsync(Initiative initiative,
            CancellationToken cancellationToken = default(CancellationToken))
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
                var contractResolver = new InitiativeContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<Initiative>(ideaString, settings);
            });
        }

        public async Task<Initiative> GetInitiativeAsync(int id)
        {
            return await ExecuteAsync(async client =>
            {
                _logger.Information("Retrieving initiative from {Url}", client.BaseAddress + id.ToString());
                var ideaString = await client.GetStringAsync(id.ToString());
                _logger.Debug("Retrieved initative {InitiativeId} data: {Text}", id, ideaString);
                var contractResolver = new InitiativeContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<Initiative>(ideaString, settings);
            });
        }

        public async Task<IEnumerable<InitiativeInfo>> GetInitiativesAsync(int pageNumber, int pageSize)
        {
            return await ExecuteAsync(async client =>
            {
				var ideaString = await client.GetStringAsync($"?page={pageNumber}&pageSize={pageSize}");
				var contractResolver = new InitiativeContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<IEnumerable<InitiativeInfo>>(ideaString, settings);
            });
        }

        public async Task<IEnumerable<InitiativeInfo>> GetInitiativesByStakeholderPersonIdAsync(int personId, int pageNumber, int pageSize)
        {
            return await ExecuteAsync(async client =>
            {
                var ideaString = await client.GetStringAsync("?View=Mine");
                var contractResolver = new InitiativeContractResolver();
                var settings = new JsonSerializerSettings() { ContractResolver = contractResolver };
                return JsonConvert.DeserializeObject<IEnumerable<InitiativeInfo>>(ideaString, settings);
            });
        }



        public Task<Initiative> UpdateInitiativeAsync(Initiative initiative)
        {
            throw new NotSupportedException();
        }

        public Task<Initiative> GetInitiativeByWorkOrderIdAsync(string workOrderId)
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

        public Task<Initiative> GetInitiativeByApexId(int apexId)
        {
            throw new NotImplementedException();
        }

		internal class InitiativeContractResolver : DefaultContractResolver
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

            //protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            //{
            //    var publicProperties = base.CreateProperties(type, memberSerialization)
            //        .ToDictionary(x => x.PropertyName, x => x);


            //    // set public properties with private setters as writable
            //    var privateProperties = type.GetProperties()
            //        .Where(p => p.GetSetMethod() != null && !p.GetSetMethod().IsPublic)
            //        .ToList();

            //    foreach (var p in privateProperties)
            //    {
            //        if (publicProperties.ContainsKey(p.Name))
            //            publicProperties[p.Name].Writable = true;
            //    }

            //    return publicProperties.Select(x => x.Value).ToList();

            //    //return publicProperties.Union(privateProperties).ToList();

            //    //var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            //    //                .Select(p => base.CreateProperty(p, memberSerialization))
            //    //            .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            //    //                       .Select(f => base.CreateProperty(f, memberSerialization)))
            //    //            .ToList();
            //    //props.ForEach(p => { p.Writable = true; p.Readable = true; });
            //    //return props;
            //}

            //protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            //{
            //    var returnValue = base.CreateProperty(member, memberSerialization);
            //    if (memberSerialization == MemberSerialization.OptOut && member is PropertyInfo)
            //    {
            //        var p = (PropertyInfo)member;
            //        var setMethod = p.GetSetMethod();
            //        if (setMethod != null && !setMethod.IsPublic)
            //        {
            //            //setMethod.Invoke()
            //        }
            //    }
            //    return returnValue;
            //}

            //protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            //{
            //    var returnValue = base.GetSerializableMembers(objectType);
            //    if (objectType == typeof(Initiative))
            //    {
            //        var moreMembers = 
            //        returnValue = returnValue.Union(
            //            typeof(Initiative)
            //                .GetProperties()
            //                .Where(p => !(p.SetMethod != null && p.SetMethod.IsPublic)))
            //            .ToList();
            //    }
            //    return returnValue;
            //}
        }

    }
}
