using AutoMapper;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal class IdeaRepositoryInternal : IIdeaRepository
    {
        private readonly IdeaContext _context;
        private readonly IMapper _mapper;
        private readonly IWordPressClient _wordpressClient;
        private readonly IIdeaServiceBusSender _serviceBusSender;
        private readonly ILogger<IdeaRepositoryInternal> _logger;

        public IdeaRepositoryInternal(IdeaContext context, 
            IMapper mapper, 
            IWordPressClient wordpressClient, 
            IIdeaServiceBusSender serviceBusSender,
            ILogger<IdeaRepositoryInternal> logger)
        {
            _context = context ?? throw new ArgumentNullException("context");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _wordpressClient = wordpressClient ?? throw new ArgumentNullException("wordpressClient");
            _serviceBusSender = serviceBusSender ?? throw new ArgumentNullException("serviceBusSender");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        #region Ideas

        private IQueryable<IdeaInternal> IdeaCollection
        {
            get
            {
                return _context.Ideas
                    .Include(x => x.Stakeholders)
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

        public async Task<Core.Idea> AddIdeaAsync(Idea idea)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");

            _logger.LogInformation("Begin AddIdeaAsync");
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch = new Stopwatch();
                watch.Start();
            }

            // get user from WordPress
            WordPressUser wpUser;
            try
            {
                wpUser = await _wordpressClient.GetCurrentUserAsync();
            }
            catch (Exception err)
            {
                throw new SecurityException($"Unable to determine current WordPress user: { err.Message }");
            }

            if (wpUser == null)
            {
                _logger.LogError("Unable to determine current WordPress user");
                throw new SecurityException("Unable to determine current WordPress user");
            }
            else if (string.IsNullOrWhiteSpace(wpUser.Email))
            {
                _logger.LogError($"Current WordPress user ({ wpUser.Name }) does not have an email");
                throw new InvalidOperationException("Current user does not have an email address registered in WordPress");
            }

            _logger.LogDebug($"Retrieved user { wpUser.Email }");

            // User must be authenticated to add new ideas.
            // We'll ensure the current user is added as an "owner" stakeholder.
            if (idea.Stakeholders == null)
                idea.Stakeholders = new List<Stakeholder>();

            var existingStakeholder = idea.Stakeholders
                .FirstOrDefault(x => wpUser.Email.Equals(x.Email, StringComparison.InvariantCultureIgnoreCase));

            if (existingStakeholder == null)
            {
                _logger.LogDebug($"Adding current user { wpUser.Email } to stakeholders list as owner");
                idea.Stakeholders.Add(new Stakeholder()
                {
                    Email = wpUser.Email,
                    UserName = wpUser.Name,
                    Type = Enum.GetName(typeof(StakeholderType), StakeholderType.Owner)
                });
            }

            var ideaInternal = _mapper.Map<Idea, IdeaInternal>(idea);

            // default values that cannot be set by users
            _logger.LogDebug("Setting default values");
            ideaInternal.Status = InitiativeStatusInternal.Initiate;
            ideaInternal.CreatedDate = DateTimeOffset.Now;

            // post to WordPress
            _logger.LogDebug("Posting to WordPress");
            var wordPressIdea = await _wordpressClient.PostIdeaAsync(idea);
            ideaInternal.WordPressKey = wordPressIdea.Id;
            ideaInternal.Url = wordPressIdea.Link;

            _logger.LogDebug("Adding to Ideas database");
            _context.Ideas.Add(ideaInternal);
            await _context.SaveChangesAsync();

            var returnValue = _mapper.Map<IdeaInternal, Idea>(ideaInternal);

            _logger.LogDebug("Posting to service bus");
            try
            {
                await _serviceBusSender.SendIdeaMessageAsync(returnValue, IdeaMessageType.IdeaCreated);
                _logger.LogDebug("Posted to service bus");
            }
            catch (Exception err)
            {
                _logger.LogError("Unable to send to service bus: {Error}", err);
                // should we throw this error???
                System.Diagnostics.Trace.TraceError($"Idea saved but there was an error sending a message to the service bus: { err.Message }");
            }

            return returnValue;
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

            var idea = await _context.Ideas.SingleOrDefaultAsync(m => m.Id == id);
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
                idea.Status =  _mapper.Map<InitiativeStatus, InitiativeStatusInternal>(status);
                await _context.SaveChangesAsync();

                return _mapper.Map<IdeaInternal, Idea>(idea);
            }
        }
        #endregion


        #region Tags

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            //TODO: retrict to a reasonable amount of tags
            var tags = await _context.Tags.ToListAsync();
            return _mapper.Map<IEnumerable<TagInternal>, IEnumerable<Tag>>(tags);
        }

        public async Task<Tag> GetTagAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var tagInternal = await _context.Tags.SingleOrDefaultAsync(m => m.Id == id);
            if (tagInternal == null)
                return null;
            else
                return _mapper.Map<TagInternal, Tag>(tagInternal);
        }

        public async Task<Tag> AddTagAsync(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");

            var tagInternal = _mapper.Map<Tag, TagInternal>(tag);
            tagInternal.CreatedDate = DateTimeOffset.Now;

            _context.Tags.Add(tagInternal);
            await _context.SaveChangesAsync();

            return _mapper.Map<TagInternal, Tag>(tagInternal);
        }

        public async Task<Tag> UpdateTagAsync(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");

            var tagInternal = _mapper.Map<Tag, TagInternal>(tag);
            _context.Entry(tagInternal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await IdeaExists(tag.Id))
                {
                    throw new EntityNotFoundException($"Tag with id {tag.Id} not found", "Tag", tag.Id);
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<TagInternal, Tag>(tagInternal);
        }

        public async Task<Tag> DeleteTagAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var tag = await _context.Tags.SingleOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return null;
            }
            else
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return _mapper.Map<TagInternal, Tag>(tag);
            }
        }


        #endregion

        #region Departments and Branches

        public async Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            var branches = await _context.Branches.ToListAsync();
            return _mapper.Map<IEnumerable<BranchInternal>, IEnumerable<Branch>>(branches);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsForBranchAsync(long branchId)
        {
            var departments = await _context.Departments
                .Include(d => d.Branch)
                .Where(x => x.Branch.Id == branchId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DepartmentInternal>, IEnumerable<Department>>(departments);
        }

        public async Task<Branch> GetBranchAsync(long id)
        {
            var branch = await _context.Branches.FindAsync(id);
            return _mapper.Map<BranchInternal, Branch>(branch);
        }

        #endregion
    }
}
