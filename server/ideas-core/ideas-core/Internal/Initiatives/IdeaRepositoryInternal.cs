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
    internal partial class IdeaRepositoryInternal : IUpdatableIdeaRepository
    {
        private readonly IdeaContext _context;
        private readonly IMapper _mapper;
        private readonly IStringTemplateService _stringTemplateService;
        private readonly ILogger<IdeaRepositoryInternal> _logger;

        public IdeaRepositoryInternal(IdeaContext context, 
            IMapper mapper,
            IStringTemplateService stringTemplateService, 
            ILogger<IdeaRepositoryInternal> logger)
        {
            _context = context ?? throw new ArgumentNullException("context");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _stringTemplateService = stringTemplateService ?? throw new ArgumentNullException("stringTemplateService");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        #region IdeaSteps

        // data should look like:
//        const steps2 = {
//data: [{
//'title': 'Submitted',
//'description': 'Thank you! Your initiative was submitted.',
//'startDate': 'Feb 13 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
//'completionDate': 'Feb 14 2018 12:23:47 GMT-0700 (Mountain Standard Time)'
//},
//{
//'title': 'In Review',
//'description': 'Your initiative has been assigned and reviewed.',
//'startDate': 'Feb 14 2018 12:23:47 GMT-0700 (Mountain Standard Time)',
//'completionDate': 'Feb 17 2018 12:23:47 GMT-0700 (Mountain Standard Time)'
//},
//{
//'title': 'In Collaboration',
//'description': 'We are actively working with you to complete an Investment Request for your initiative.',
//'startDate': 'Feb 17 2018 12:23:47 GMT-0700 (Mountain Standard Time)',
//'completionDate': null
//},
//{
//'title': 'In Delivery',
//'description': 'Pellentesque ut neque tempus, placerat purus volutpat, scelerisque velit. Vivamus porta urna vel ligula lobortis, id porttitor quam maximus.',
//'startDate': null,
//'completionDate': null
//}
//]
//}

        public async Task<IEnumerable<IdeaStep>> GetInitiativeStepsAsync(long initiativeId)
        {
            var initiative = await _context.Ideas.FindAsync(initiativeId);
            if (initiative == null)
                throw new EntityNotFoundException($"Unable to find an initiative with id " + initiativeId);

            var statusHistories = await _context.IdeaStatusHistories
                .Include(x => x.Assignee)
                .Where(x => x.Initiative == initiative)
                .ToListAsync();

            if (!statusHistories.Any())
                return new IdeaStep[] { };

            // get the latest
            var latest = statusHistories.OrderByDescending(x => x.StatusEntryDateUtc).First();

            // discard any entries that are after this step in the workflow
            statusHistories = statusHistories.Where(x => (int)x.Status <= (int)latest.Status).ToList();

            var items = statusHistories.GroupBy(x => x.Status)
                .ToDictionary(x => x.Key, 
                y => new
                {
                    EnterItem = y.OrderBy(z => z.StatusEntryDateUtc).First(),
                    ExitItem = y.OrderByDescending(z => z.StatusEntryDateUtc).First(),
                    Items = y
                });

            // Get the distinct statuses, and the first and last entries where the initiative entered
            // each status (if if re-enters statuses)
            var steps = items.Select(x => new 
            {
                Title = GetInitiativeStepsAsync_GetTitle(x.Key),
                Description = x.Value.ExitItem.Text,
                StartDate = (DateTime?)x.Value.ExitItem.StatusEntryDateUtc,
                CompletionDate = (DateTime?)items.Where(y => y.Value.EnterItem.StatusEntryDateUtc > x.Value.ExitItem.StatusEntryDateUtc)
                                      .OrderBy(y => y.Value.EnterItem.StatusEntryDateUtc)
                                      .Select(y => y.Value.EnterItem.StatusEntryDateUtc)
                                      .FirstOrDefault(),
                stepOrder = (int)x.Key
            })
            .OrderBy(x => x.StartDate)
            .ToList();


            // finally, we set any missing steps so the front end get all available steps
            // note, it's not really all steps, just the ones we really care about on our screen
            var allStatuses = new InitiativeStatusInternal[] { InitiativeStatusInternal.Submit, InitiativeStatusInternal.Review, InitiativeStatusInternal.Collaborate, InitiativeStatusInternal.Deliver };
            var missingStatuses = allStatuses
                .Where(x => !items.Any(y => y.Key == x))
                .Select(x => new
                {
                    Title = GetInitiativeStepsAsync_GetTitle(x),
                    Description = (string)null,
                    StartDate = (DateTime?)null,
                    CompletionDate = (DateTime?)null,
                    stepOrder = (int)x
                })
                .ToList();

            var returnValue = steps.Union(missingStatuses)
                .OrderBy(x => x.stepOrder)
                .Select(x => new IdeaStep()
                {
                    Title = x.Title,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    
                    //bug fix for above where CompletionDate is default(DateTimeOffset)
                    CompletionDate = x.CompletionDate.HasValue && x.CompletionDate.Value.Ticks == 0 ? null : x.CompletionDate
                })
                .ToList();

            return returnValue;
        }

        private string GetInitiativeStepsAsync_GetTitle(InitiativeStatusInternal status)
        { 
            switch (status)
            {
                case InitiativeStatusInternal.Initiate:
                    return "Initiated";
                case InitiativeStatusInternal.Submit:
                    return "Submitted";
                case InitiativeStatusInternal.Review:
                    return "In Review";
                case InitiativeStatusInternal.Collaborate:
                    return "In Collaboration";
                case InitiativeStatusInternal.Deliver:
                    return "In Delivery";
                case InitiativeStatusInternal.Cancelled:
                    return "Cancelled";
                default:
                    return status.ToString();
            }
        }

        #endregion


        #region People

        public async Task<Person> GetPersonByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("email");
            string emailUpper = email.ToUpperInvariant();
            var person = await _context.People.FirstOrDefaultAsync(x => x.Email.ToUpperInvariant() == emailUpper);
            return _mapper.Map<PersonInternal, Person>(person);
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
