using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal partial class IdeaRepositoryInternal : IUpdatableIdeaRepository
    {
        public async Task<Stakeholder> GetStakeholderByEmailAsync(string email)
        {
            var stakeholders = await _context.Stakeholders.FirstOrDefaultAsync(x => x.Email == email);
            return _mapper.Map<StakeholderInternal, Stakeholder>(stakeholders);
        }
    }
}
