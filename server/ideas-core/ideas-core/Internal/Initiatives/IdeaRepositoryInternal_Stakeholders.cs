using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal partial class IdeaRepositoryInternal : IUpdatableIdeaRepository
    {
        public Task<Stakeholder> GetStakeholderByEmailAsync(string email)
        {
            return _context.Stakeholders.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
