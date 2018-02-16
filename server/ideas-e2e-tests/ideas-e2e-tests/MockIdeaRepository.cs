using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockIdeaRepository : IIdeaRepository
    {
        public MockIdeaRepository(IIdeaServiceBusSender serviceBusSender)
        {
            _serviceBusSender = serviceBusSender ?? throw new ArgumentNullException("serviceBusSender");
        }
        private IIdeaServiceBusSender _serviceBusSender;

        #region Ideas
        private ICollection<Idea> ideas = new HashSet<Idea>();

        public Task<Idea> AddIdeaAsync(Idea idea)
        {
            ideas.Add(idea);
            idea.Id = ideas.Count;
            idea.WordPressKey = (int)idea.Id;

            _serviceBusSender.SendIdeaMessageAsync(idea, IdeaMessageType.IdeaCreated);
            return Task.FromResult(idea); ;
        }
        public Task<IEnumerable<Idea>> GetIdeasAsync()
        {
            return Task.FromResult(ideas.AsEnumerable());
        }

        public Task<IEnumerable<Idea>> GetIdeasByStakeholderAsync(long stakeholderId)
        {
            throw new NotImplementedException();
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

        #endregion
    }
}
