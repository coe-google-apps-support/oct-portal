using AutoMapper;
using CoE.Ideas.Core.WordPress;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal
{
    internal class IdeaRepositoryInternal : IIdeaRepository
    {
        private readonly IdeaContext _context;
        private readonly IMapper _mapper;
        private readonly IWordPressClient _wordpressClient;

        public IdeaRepositoryInternal(IdeaContext context, IMapper mapper, IWordPressClient wordpressClient)
        {
            _context = context;
            _mapper = mapper;
            _wordpressClient = wordpressClient;
        }


        public async Task<Core.Idea> AddIdeaAsync(Idea idea)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");

            // get user from WordPress
            var wpUser = await _wordpressClient.GetCurrentUserAsync();

            if (wpUser == null)
            {
                throw new SecurityException("Unable to determine current WordPress user");
            }
            else if (string.IsNullOrWhiteSpace(wpUser.Email))
            {
                throw new InvalidOperationException("Current user does not have an email address registered in WordPress");
            }

            // User must be authenticated to add new ideas.
            // We'll ensure the current user is added as an "owner" stakeholder.
            if (idea.Stakeholders == null)
                idea.Stakeholders = new List<Stakeholder>();

            var existingStakeholder = idea.Stakeholders
                .FirstOrDefault(x => wpUser.Email.Equals(x.Email, StringComparison.InvariantCultureIgnoreCase));

            if (existingStakeholder == null)
            {
                idea.Stakeholders.Add(new Stakeholder()
                {
                    Email = wpUser.Email,
                    UserName = wpUser.Name,
                    Type = StakeholderType.Owner
                });
            }

            var ideaInternal = _mapper.Map<Idea, IdeaInternal>(idea);

            // post to WordPress
            var wordPressIdea = await _wordpressClient.PostIdeaAsync(idea);
            ideaInternal.WordPressKey = wordPressIdea.Id;

            _context.Ideas.Add(ideaInternal);
            await _context.SaveChangesAsync();

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

        public async Task<Core.Idea> GetIdeaAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var ideaInternal = await _context.Ideas.SingleOrDefaultAsync(m => m.Id == id);
            if (ideaInternal == null)
                return null;
            else
                return _mapper.Map<IdeaInternal, Idea>(ideaInternal);
        }

        public async Task<IEnumerable<Core.Idea>> GetIdeasAsync()
        {
            //TODO: retrict to a reasonable amount of ideas
            var ideas = await _context.Ideas.Include(x => x.Stakeholders).ToListAsync();
            return _mapper.Map<IEnumerable<IdeaInternal>, IEnumerable<Idea>>(ideas);

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


        private async Task<bool> IdeaExists(long id)
        {
            return await _context.Ideas.AnyAsync(e => e.Id == id);
        }
    }
}
