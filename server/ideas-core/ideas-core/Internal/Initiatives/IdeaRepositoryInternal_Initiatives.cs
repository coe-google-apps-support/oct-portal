using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal partial class IdeaRepositoryInternal : IUpdatableIdeaRepository
    {
        private IQueryable<IdeaInternal> IdeaCollection
        {
            get
            {
                return _context.Ideas
                    .Include(x => x.Stakeholders)
                    .ThenInclude(y => y.Person)
                    .Include(x => x.Assignee)
                    .Include(x => x.Tags)
                    .Include(x => x.Department)
                    .ThenInclude(y => y.Branch);
            }
        }

        private async Task<bool> IdeaExists(long id)
        {
            return await _context.Ideas.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Core.Idea>> GetIdeasAsync()
        {
            //TODO: retrict to a reasonable amount of ideas
            var ideas = await IdeaCollection.ToListAsync();
            return _mapper.Map<IEnumerable<IdeaInternal>, IEnumerable<Idea>>(ideas);
        }

        public async Task<Core.Idea> GetIdeaAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var ideaInternal = await IdeaCollection.SingleOrDefaultAsync(m => m.Id == id);
            if (ideaInternal == null)
                return null;
            else
                return _mapper.Map<IdeaInternal, Idea>(ideaInternal);
        }

        public async Task<Idea> GetIdeaByWordpressKeyAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var ideaInternal = await IdeaCollection.SingleOrDefaultAsync(m => m.WordPressKey == id);
            if (ideaInternal == null)
                return null;
            else
                return _mapper.Map<IdeaInternal, Idea>(ideaInternal);
        }

        public async Task<Idea> GetIdeaByWorkItemIdAsync(string workItemId)
        {
            if (string.IsNullOrWhiteSpace(workItemId))
            {
                throw new ArgumentNullException("workItemId");
            }

            var ideaInternal = await IdeaCollection.SingleOrDefaultAsync(m => m.WorkItemId == workItemId);
            if (ideaInternal == null)
                return null;
            else
                return _mapper.Map<IdeaInternal, Idea>(ideaInternal);
        }

        public async Task<Core.Idea> AddIdeaAsync(Idea idea, System.Security.Claims.ClaimsPrincipal owner)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");
            if (owner == null)
                throw new ArgumentNullException("owner");

            string ownerEmail = owner.GetEmail();
            if (string.IsNullOrWhiteSpace(ownerEmail))
                throw new InvalidOperationException("Unable to get email address from owner");

            _logger.LogInformation("Begin AddIdeaAsync");
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch = new Stopwatch();
                watch.Start();
            }


            // User must be authenticated to add new ideas.
            // We'll ensure the current user is added as an "owner" stakeholder.
            if (idea.Stakeholders == null)
                idea.Stakeholders = new List<Stakeholder>();

            var existingPerson = _context.People
                .FirstOrDefault(x => ownerEmail.Equals(x.Email, StringComparison.InvariantCultureIgnoreCase));

            if (existingPerson == null)
            {
                existingPerson = new PersonInternal() { Email = ownerEmail, UserName = owner.GetDisplayName() };
            }
            var ownerPerson = _mapper.Map<PersonInternal, Person>(existingPerson);

            var existingStakeholder = idea.Stakeholders
                .FirstOrDefault(x => x.Email == ownerEmail || x.Person?.Email == ownerEmail);
            if (existingStakeholder == null)
            {
                _logger.LogDebug($"Adding current user { ownerEmail } to stakeholders list as owner");
                idea.Stakeholders.Add(new Stakeholder() { Person = ownerPerson, Type = "owner" });
            }

            var ideaInternal = _mapper.Map<Idea, IdeaInternal>(idea);

            // default values that cannot be set by users
            _logger.LogDebug("Setting default values");
            ideaInternal.Status = InitiativeStatusInternal.Initiate;
            ideaInternal.CreatedDate = DateTimeOffset.Now;

            _logger.LogDebug("Adding to Ideas database");
            _context.Ideas.Add(ideaInternal);
            await _context.SaveChangesAsync();

            var returnValue = _mapper.Map<IdeaInternal, Idea>(ideaInternal);
            return returnValue;
        }

        public async Task<Idea> SetWordPressItemAsync(long ideaId, Core.WordPress.WordPressPost post)
        {
            if (ideaId <= 0)
                throw new ArgumentOutOfRangeException("ideaId", "ideaId cannot be less than or equal to zero");
            if (post == null)
                throw new ArgumentNullException("post");

            var idea = await _context.Ideas.FindAsync(ideaId);

            if (idea.WordPressKey > 0)
                throw new InvalidOperationException($"WorkPressKey is already set for idea with id { ideaId }: { idea.WordPressKey }");

            idea.WordPressKey = post.Id;
            idea.Url = post.Link;

            await _context.SaveChangesAsync();

            return _mapper.Map<Idea>(idea);
        }

        public async Task<Core.Idea> UpdateIdeaAsync(Core.Idea idea)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");

            var ideaInternal = _mapper.Map<Idea, IdeaInternal>(idea);
            _context.Entry(ideaInternal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await IdeaExists(idea.Id))
                {
                    throw new EntityNotFoundException($"Idea with id {idea.Id} not found", "Idea", idea.Id);
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<IdeaInternal, Idea>(ideaInternal);
        }

        public async Task<Core.Idea> DeleteIdeaAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var idea = await _context.Ideas.FindAsync(id);
            if (idea == null)
            {
                return null;
            }
            else
            {
                _context.Ideas.Remove(idea);
                await _context.SaveChangesAsync();

                return _mapper.Map<IdeaInternal, Idea>(idea);
            }
        }

        public async Task<Idea> SetWorkItemTicketIdAsync(long id, string workItemId)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");
            if (string.IsNullOrWhiteSpace(workItemId))
                throw new ArgumentNullException("workItemId");

            var idea = await _context.Ideas.SingleOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return null;
            }
            else
            {
                idea.WorkItemId = workItemId;
                await _context.SaveChangesAsync();

                return _mapper.Map<IdeaInternal, Idea>(idea);
            }
        }

        public async Task<Idea> SetWorkItemStatusAsync(long id, InitiativeStatus status)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var idea = await _context.Ideas.SingleOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return null;
            }
            else
            {
                idea.Status = _mapper.Map<InitiativeStatus, InitiativeStatusInternal>(status);
                await _context.SaveChangesAsync();

                return _mapper.Map<IdeaInternal, Idea>(idea);
            }
        }


        public async Task<IEnumerable<Idea>> GetIdeasByStakeholderEmailAsync(string emailAddress)
        {
            var ideas = await IdeaCollection.Where(x => x.Stakeholders.Any(y => y.Person.Email == emailAddress)).ToListAsync();
            return _mapper.Map<IEnumerable<IdeaInternal>, IEnumerable<Idea>>(ideas);
        }

        public async Task<Idea> SetInitiativeAssignee(long ideaId, Person person)
        {
            var idea = await _context.Ideas.FindAsync(ideaId);
            if (idea == null)
                throw new InvalidOperationException($"Unable to find an initiative with id { ideaId }");

            bool handled = false;
            if (person == null)
            {
                // easiest case!
                idea.Assignee = null;
                handled = true;
            }
            else
            {
                if (person.Id > 0)
                {
                    var existingPerson = await _context.People.FindAsync(person.Id);
                    if (existingPerson != null)
                    {
                        idea.Assignee = existingPerson;
                        handled = true;
                    }
                }
            }

            if (!handled)
            {
                idea.Assignee = _mapper.Map<Person, PersonInternal>(person);
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<IdeaInternal, Idea>(idea);

        }
    }
}
