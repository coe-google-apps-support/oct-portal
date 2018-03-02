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
                .ToDictionary(x => x.Key, y => new { EnterDate = y.Min(z => z.StatusEntryDateUtc), ExitDate = y.Max(z => z.StatusEntryDateUtc), Items = y });

            // mapping of history records and their completedDate
            var cleanedSteps = new Dictionary<IdeaStatusHistoryInternal, DateTimeOffset>(); 

            DateTime lastStepExitDate = DateTime.MaxValue.ToUniversalTime();
            foreach (var statusGrouping in items.OrderByDescending(x => (int)x.Key))
            {
                var item = statusGrouping.Value.Items
                    .Where(x => x.StatusEntryDateUtc < lastStepExitDate)
                    .OrderByDescending(x => x.StatusEntryDateUtc)
                    .FirstOrDefault();
                if (item != null)
                {
                    var exitDateUtc = DateTime.SpecifyKind(statusGrouping.Value.ExitDate, DateTimeKind.Utc);
                    cleanedSteps.Add(item, exitDateUtc.ToLocalTime());
                    lastStepExitDate = statusGrouping.Value.ExitDate;
                }
            }

            // translate cleanedSteps to IdeaStep
            var returnValue = cleanedSteps.Select(x => new IdeaStep()
            {
                Name = x.Key.Status.ToString(),
                CompletedDate = x.Value,
                Step = (int)x.Key.Status - 2,
                Type = GetInitiativeStepsAsync_GetType(x.Key),
                Data = GetInitiativeStepsAsync_GetData(x.Key)
            });

            return returnValue.OrderBy(x => x.CompletedDate);
        }

        private string GetInitiativeStepsAsync_GetType(IdeaStatusHistoryInternal historyItem)
        {
            switch (historyItem.Status)
            {
                case InitiativeStatusInternal.Submit:
                    return "text";
                case InitiativeStatusInternal.Review:
                    return "resource";
                case InitiativeStatusInternal.Collaborate:
                    return "resource";
                case InitiativeStatusInternal.Deliver:
                    return "burndown";
                default:
                    return "text";
            }
           
        }

        private object GetInitiativeStepsAsync_GetData(IdeaStatusHistoryInternal historyItem)
        {
            if (historyItem.Status == InitiativeStatusInternal.Review || historyItem.Status == InitiativeStatusInternal.Collaborate)
            {
                // these are the "review" steps
                var people = new List<object>();
                if (historyItem.Assignee != null)
                {
                    people.Add(new
                    {
                        User = historyItem.Assignee.Email,
                        AssignedOn = (DateTimeOffset)historyItem.StatusEntryDateUtc.ToLocalTime(),
                        AvatarUrl = string.Empty
                    });
                }
                return people;
            }
            else if (historyItem.Status == InitiativeStatusInternal.Deliver)
            {
                // this is of type "BurnDown"
                return Newtonsoft.Json.Linq.JObject.Parse(fakeBurndown);
            }
            else
            {
                // this is just the text
                return historyItem.Text;
            }
        }

        private const string fakeBurndown = @"
[
      {
        date: 'Jan 18 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 0,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-18..2018-01-18&type=Issues'
      },
      {
        date: 'Jan 19 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 2,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-19..2018-01-19&type=Issues'
      },
      {
        date: 'Jan 20 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-20..2018-01-20&type=Issues'
      },
      {
        date: 'Jan 21 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-21..2018-01-21&type=Issues'
      },
      {
        date: 'Jan 22 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-22..2018-01-22&type=Issues'
      },
      {
        date: 'Jan 23 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-23..2018-01-23&type=Issues'
      },
      {
        date: 'Jan 24 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-24..2018-01-24&type=Issues'
      },
      {
        date: 'Jan 25 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-25..2018-01-25&type=Issues'
      },
      {
        date: 'Jan 26 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-26..2018-01-26&type=Issues'
      },
      {
        date: 'Jan 27 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-27..2018-01-27&type=Issues'
      },
      {
        date: 'Jan 28 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-28..2018-01-28&type=Issues'
      },
      {
        date: 'Jan 29 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 23,
        workRemoved: 0,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-29..2018-01-29&type=Issues'
      },
      {
        date: 'Jan 30 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 1,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-30..2018-01-30&type=Issues'
      },
      {
        date: 'Jan 31 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 5,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-31..2018-02-31&type=Issues'
      }
    ]";
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
