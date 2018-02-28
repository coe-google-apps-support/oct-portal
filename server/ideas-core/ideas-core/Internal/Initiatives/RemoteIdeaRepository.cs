using CoE.Ideas.Core.Internal.WordPress;
using CoE.Ideas.Core.Security;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal class RemoteIdeaRepository : IIdeaRepository
    {

        public RemoteIdeaRepository(
            ClaimsPrincipal user,
            IWordPressUserSecurity wordPressUserSecurity,
            IOptions<RemoteIdeaRepositoryOptions> options,
            Serilog.ILogger logger)
        {
            _user = user ?? throw new ArgumentNullException("user");
            _wordPressUserSecurity = wordPressUserSecurity ?? throw new ArgumentNullException("wordPressUserSecurity");
            _logger = logger ?? throw new ArgumentNullException("logger");

            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            if (string.IsNullOrWhiteSpace(options.Value.Url))
                throw new ArgumentNullException("Url");

            if (options.Value.Url.EndsWith("/"))
                _baseUri = new Uri(options.Value.Url);
            else
                _baseUri = new Uri(options.Value.Url + "/");
        }

        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly Uri _baseUri;
        private readonly Serilog.ILogger _logger;

        private readonly ClaimsPrincipal _user;

        private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = _baseUri;
                _wordPressUserSecurity.SetWordPressCredentials(client, cookieContainer, _user);
                return await callback(client);
            }
        }


        #region Ideas
        public async Task<IEnumerable<Idea>> GetIdeasAsync()
        {
            return await ExecuteAsync(async client =>
            {
                try
                {
                    var allIdeasString = await client.GetStringAsync(string.Empty);
                    return JsonConvert.DeserializeObject<IEnumerable<Idea>>(allIdeasString);

                }
                catch (Exception err)
                {
                    throw err;
                }
            });
        }


        public Task<IEnumerable<Idea>> GetIdeasByStakeholderEmailAsync(string stakeholderEmail)
        {
            throw new NotSupportedException();
        }

        public async Task<Idea> GetIdeaAsync(long id)
        {
            return await ExecuteAsync(async client =>
            {
                try
                {
                    var ideaString = await client.GetStringAsync(id.ToString());
                    return JsonConvert.DeserializeObject<Idea>(ideaString);

                }
                catch (Exception err)
                {
                    throw err;
                }
            });
        }

        public Task<Idea> GetIdeaByWordpressKeyAsync(int id)
        {
            throw new NotSupportedException();
        }
        public Task<Idea> GetIdeaByWorkItemIdAsync(string workItemId)
        {
            throw new NotSupportedException();
        }

        public Task<Idea> AddIdeaAsync(Idea idea)
        {
            throw new NotSupportedException();
        }

        public Task<Idea> UpdateIdeaAsync(Idea idea)
        {
            throw new NotSupportedException();
        }

        public Task<Idea> DeleteIdeaAsync(long id)
        {
            throw new NotSupportedException();
        }
        public Task<Idea> SetWorkItemTicketIdAsync(long id, string workItemId)
        {
            throw new NotSupportedException();
        }

        public Task<Idea> SetWorkItemStatusAsync(long id, InitiativeStatus status)
        {
            throw new NotSupportedException();

        }
        #endregion

        #region People

        public Task<Person> GetPersonByEmail(string email)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Tags
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await ExecuteAsync(async client =>
            {
                try
                {
                    var allIdeasString = await client.GetStringAsync(_baseUri + "/tags");
                    return JsonConvert.DeserializeObject<IEnumerable<Tag>>(allIdeasString);
                }
                catch (Exception err)
                {
                    throw err;
                }

            });
        }

        public Task<Tag> GetTagAsync(long id)
        {
            throw new NotSupportedException();
        }

        public Task<Tag> UpdateTagAsync(Tag tag)
        {
            throw new NotSupportedException();
        }

        public Task<Tag> AddTagAsync(Tag tag)
        {
            throw new NotSupportedException();
        }

        public Task<Tag> DeleteTagAsync(long id)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Branches and Departments

        public Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Department>> GetDepartmentsForBranchAsync(long branchId)
        {
            throw new NotImplementedException();
        }

        public Task<Branch> GetBranchAsync(long id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
