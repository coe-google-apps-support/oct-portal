using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockIdeaRepository : IUpdatableIdeaRepository
    {
        //public MockIdeaRepository(IIdeaServiceBusSender serviceBusSender)
        //{
        //    _serviceBusSender = serviceBusSender ?? throw new ArgumentNullException("serviceBusSender");
        //}
        //private IIdeaServiceBusSender _serviceBusSender;

        #region Ideas
        private ICollection<Idea> ideas = new HashSet<Idea>();

        public Task<Idea> AddIdeaAsync(Idea idea, Stakeholder owner)
        {
            ideas.Add(idea);
            idea.Id = ideas.Count;

            if (idea.Stakeholders == null)
                idea.Stakeholders = new List<Stakeholder>();

            if (!idea.Stakeholders.Contains(owner))
                idea.Stakeholders.Add(owner);

            return Task.FromResult(idea); ;
        }
        public Task<IEnumerable<Idea>> GetIdeasAsync()
        {
            return Task.FromResult(ideas.AsEnumerable());
        }


        public Task<Idea> GetIdeaAsync(long id)
        {
            return Task.FromResult(ideas.FirstOrDefault(x => x.Id == id));
        }

        public Task<Idea> GetIdeaByWordpressKeyAsync(int id)
        {
            return Task.FromResult(ideas.FirstOrDefault(x => x.WordPressKey == id));
        }

        public Task<Idea> GetIdeaByWorkItemIdAsync(string workItemId)
        {
            return Task.FromResult(ideas.FirstOrDefault(x => x.WorkItemId == workItemId));
        }

        public Task<Idea> UpdateIdeaAsync(Idea idea)
        {
            // nothing to do because we're updating ideas directly
            return Task.FromResult(idea);
        }

        public Task<Idea> DeleteIdeaAsync(long id)
        {
            var ideasToDelete = ideas.Where(x => x.Id == id).ToList();
            foreach (var idea in ideasToDelete)
            {
                ideas.Remove(idea);
            }
            return Task.FromResult(ideasToDelete.FirstOrDefault());
        }

        public Task<Idea> SetWorkItemStatusAsync(long id, InitiativeStatus status)
        {
            var idea = ideas.FirstOrDefault(x => x.Id == id);
            if (idea != null)
            {
                // Not a public setter. We'll do this for now.
                // Maybe we'll set InternalVisibleTo later...
                idea.GetType().GetProperty(nameof(Idea.Status))
                    .GetSetMethod(true)
                    .Invoke(idea, new object[] { status });
            }
            return Task.FromResult(idea);
        }

        public Task<Idea> SetWorkItemTicketIdAsync(long id, string workItemId)
        {
            var idea = ideas.FirstOrDefault(x => x.Id == id);
            if (idea != null)
            {
                // Not a public setter. We'll do this for now.
                // Maybe we'll set InternalVisibleTo later...
                idea.GetType().GetProperty(nameof(Idea.WorkItemId))
                    .GetSetMethod(true)
                    .Invoke(idea, new object[] { workItemId });
            }
            return Task.FromResult(idea);
        }

        #endregion

        #region Tags
        public Task<Tag> AddTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetTagAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tag>> GetTagsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Tag> UpdateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }


        public Task<Tag> DeleteTagAsync(long id)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Branches and Departments
        public Task<Branch> GetBranchAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Department>> GetDepartmentsForBranchAsync(long branchId)
        {
            throw new NotImplementedException();
        }

        public Task<Idea> SetWordPressItemAsync(long ideaId, WordPressPost post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            var idea = ideas.FirstOrDefault(x => x.Id == ideaId);
            if (idea == null)
                throw new InvalidOperationException($"Unable to find an idea with id { ideaId }");

            // unfortunately not public setter
            idea.GetType().GetProperty("WordPressKey").GetSetMethod(true).Invoke(idea, new object[] { post.Id });

            idea.Url = post.Link;

            return Task.FromResult(idea);
        }

        public Task<Stakeholder> GetStakeholderByEmailAsync(string email)
        {
            return Task.FromResult(ideas.SelectMany(x => x.Stakeholders).FirstOrDefault(x => x.Email == email));
        }
        #endregion
    }
}
