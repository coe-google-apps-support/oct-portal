using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal
{
    internal class RemoteIdeaRepository : IIdeaRepository
    {

        public RemoteIdeaRepository(
            string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(url);

            _baseUri = new Uri(url);
        }

        private readonly Uri _baseUri;

        protected virtual HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            //// easy case - we have a bearer token in our own HTTP Request headers:
            //// so we can just reuse it because WordPress should be using the same
            //// JWT Auth keys we are.
            //if (!string.IsNullOrWhiteSpace(_jwtBearerToken))
            //{
            //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtBearerToken);

            //}

            return client;
        }


        #region Ideas
        public async Task<IEnumerable<Idea>> GetIdeasAsync()
        {
            var client = GetHttpClient();

            try
            {
                var allIdeasString = await client.GetStringAsync(_baseUri);
                return JsonConvert.DeserializeObject<IEnumerable<Idea>>(allIdeasString);

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<Idea> GetIdeaAsync(long id)
        {
            var client = GetHttpClient();

            try
            {
                var ideaString = await client.GetStringAsync(_baseUri.ToString() + "initiatives/" + id);
                return JsonConvert.DeserializeObject<Idea>(ideaString);

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public Task<Idea> GetIdeaByWordpressKeyAsync(int id)
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
        #endregion

        #region Tags
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            var client = GetHttpClient();

            try
            {
                var allIdeasString = await client.GetStringAsync(_baseUri + "/tags");
                return JsonConvert.DeserializeObject<IEnumerable<Tag>>(allIdeasString);
            }
            catch (Exception err)
            {
                throw err;
            }
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
