using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal interface IStringTemplateService
    {
        Task<string> GetStatusChangeTextAsync(InitiativeStatusInternal status, PersonInternal assignee, bool isPastTense = false);
    }
}
