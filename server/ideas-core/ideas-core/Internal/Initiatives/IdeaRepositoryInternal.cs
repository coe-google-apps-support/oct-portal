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
        //private readonly IWordPressClient _wordpressClient;
        private readonly ILogger<IdeaRepositoryInternal> _logger;

        public IdeaRepositoryInternal(IdeaContext context, 
            IMapper mapper, 
           // IWordPressClient wordpressClient, 
            ILogger<IdeaRepositoryInternal> logger)
        {
            _context = context ?? throw new ArgumentNullException("context");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            //_wordpressClient = wordpressClient ?? throw new ArgumentNullException("wordpressClient");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        #region Ideas


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
