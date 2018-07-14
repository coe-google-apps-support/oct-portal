using CoE.Ideas.Shared.People;
using CoE.Issues.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy
{
    public interface IRemedyService
    {
        Task<string> PostNewissueAsync(Issue issue, PersonData personData);

    }
}
