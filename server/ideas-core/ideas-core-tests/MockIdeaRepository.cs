using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
{
    internal class MockIdeaRepository : IInitiativeRepository
    {

        #region Initiatives
        private ICollection<Initiative> initiatives = new HashSet<Initiative>();

        public Task<Initiative> AddInitiativeAsync(Initiative initiative)
        {
            initiatives.Add(initiative);
            return Task.FromResult(initiative);
        }

        public Task<Initiative> GetInitiativeAsync(Guid id)
        {
            return Task.FromResult(initiatives.Single(x => x.Id == id));
        }

        public Task<Initiative> GetInitiativeAsync(int id)
        {
            return Task.FromResult(initiatives.Single(x => x.AlternateKey == id));
        }

        public Task<Initiative> GetInitiativeByWordpressKeyAsync(int wordpressKey)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InitiativeInfo>> GetInitiativesAsync()
        {
            throw new NotImplementedException();
            //return Task.FromResult(initiatives.Select(x => new InitiativeInfo() { Id = x.Id, Title = x.Title, Description = x.Description }));
        }

        public Task<IEnumerable<InitiativeInfo>> GetInitiativesByStakeholderPersonIdAsync(int personId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InitiativeStep>> GetInitiativeStepsAsync(int initiativeId)
        {
            throw new NotImplementedException();
        }

        public Task<Initiative> UpdateInitiativeAsync(Initiative initiative)
        {
            throw new NotImplementedException();
        }


        //public Task<Initiative> AddIdeaAsync(Initiative idea, ClaimsPrincipal owner)
        //{
        //    ideas.Add(idea);
        //    idea.Id = ideas.Count;

        //    if (idea.Stakeholders == null)
        //        idea.Stakeholders = new List<Stakeholder>();

        //    string ownerEmail = owner.GetEmail();
        //    if (!idea.Stakeholders.Any(x => x.Email == ownerEmail || x.Person?.Email == ownerEmail))
        //    {
        //        idea.Stakeholders.Add(new Stakeholder()
        //        {
        //            Person = new Person()
        //            {
        //                Email = ownerEmail,
        //                UserName = owner.GetDisplayName()
        //            },
        //            Email = ownerEmail,
        //            UserName = owner.GetDisplayName(),
        //            Type = "owner"
        //        });
        //    }

        //    return Task.FromResult(idea); ;
        //}
        //public Task<IEnumerable<Idea>> GetIdeasAsync()
        //{
        //    return Task.FromResult(ideas.AsEnumerable());
        //}

        //public Task<IEnumerable<Idea>> GetIdeasByStakeholderEmailAsync(string emailAddress)
        //{
        //    return Task.FromResult(ideas.Where(x => x.Stakeholders.Any(y => y.Email == emailAddress || y.Person?.Email == emailAddress)).AsEnumerable());
        //}

        //public Task<Idea> GetIdeaAsync(long id)
        //{
        //    return Task.FromResult(ideas.FirstOrDefault(x => x.Id == id));
        //}

        //public Task<Idea> GetIdeaByWordpressKeyAsync(int id)
        //{
        //    return Task.FromResult(ideas.FirstOrDefault(x => x.WordPressKey == id));
        //}

        //public Task<Idea> GetIdeaByWorkItemIdAsync(string workItemId)
        //{
        //    return Task.FromResult(ideas.FirstOrDefault(x => x.WorkItemId == workItemId));
        //}

        //public Task<Idea> UpdateIdeaAsync(Idea idea)
        //{
        //    // nothing to do because we're updating ideas directly
        //    return Task.FromResult(idea);
        //}

        //public Task<Idea> DeleteIdeaAsync(long id)
        //{
        //    var ideasToDelete = ideas.Where(x => x.Id == id).ToList();
        //    foreach (var idea in ideasToDelete)
        //    {
        //        ideas.Remove(idea);
        //    }
        //    return Task.FromResult(ideasToDelete.FirstOrDefault());
        //}

        //public Task<Idea> SetWorkItemStatusAsync(long id, InitiativeStatus status)
        //{
        //    var idea = ideas.FirstOrDefault(x => x.Id == id);
        //    if (idea != null)
        //    {
        //        // Not a public setter. We'll do this for now.
        //        // Maybe we'll set InternalVisibleTo later...
        //        idea.GetType().GetProperty(nameof(Idea.Status))
        //            .GetSetMethod(true)
        //            .Invoke(idea, new object[] { status });
        //    }
        //    return Task.FromResult(idea);
        //}

        //public Task<Idea> SetWorkItemTicketIdAsync(long id, string workItemId)
        //{
        //    var idea = ideas.FirstOrDefault(x => x.Id == id);
        //    if (idea != null)
        //    {
        //        // Not a public setter. We'll do this for now.
        //        // Maybe we'll set InternalVisibleTo later...
        //        idea.GetType().GetProperty(nameof(Idea.WorkItemId))
        //            .GetSetMethod(true)
        //            .Invoke(idea, new object[] { workItemId });
        //    }
        //    return Task.FromResult(idea);
        //}


        //public Task<Idea> SetInitiativeAssignee(long ideaId, Person person)
        //{
        //    var idea = ideas.FirstOrDefault(x => x.Id == ideaId);
        //    if (idea != null)
        //    {
        //        idea.Assignee = person;
        //    }
        //    return Task.FromResult(idea);
        //}

        #endregion

        //#region People
        //public Task<Person> GetPersonByEmail(string email)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion

        //#region Tags
        //public Task<Tag> AddTagAsync(Tag tag)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Tag> GetTagAsync(long id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Tag>> GetTagsAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Tag> UpdateTagAsync(Tag tag)
        //{
        //    throw new NotImplementedException();
        //}


        //public Task<Tag> DeleteTagAsync(long id)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion


        //#region Branches and Departments
        //public Task<Branch> GetBranchAsync(long id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Branch>> GetBranchesAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Department>> GetDepartmentsForBranchAsync(long branchId)
        //{
        //    throw new NotImplementedException();
        //}
        //public Task<Idea> SetWordPressItemAsync(long ideaId, WordPressPost post)
        //{
        //    if (post == null)
        //        throw new ArgumentNullException("post");

        //    var idea = ideas.FirstOrDefault(x => x.Id == ideaId);
        //    if (idea == null)
        //        throw new InvalidOperationException($"Unable to find an idea with id { ideaId }");

        //    // unfortunately not public setter
        //    idea.GetType().GetProperty("WordPressKey").GetSetMethod(true).Invoke(idea, new object[] { post.Id });

        //    idea.Url = post.Link;

        //    return Task.FromResult(idea);
        //}

        //public Task<IEnumerable<IdeaStep>> GetInitiativeStepsAsync(long initiativeId)
        //{
        //    throw new NotImplementedException();
        //}




        //#endregion

    }
}
