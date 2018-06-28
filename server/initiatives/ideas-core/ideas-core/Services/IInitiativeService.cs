using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Services
{
    public interface IInitiativeService
    {
        Uri GetInitiativeUrl(int initiativeId);
    }
}
