using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;

namespace CoE.Ideas.Integration.Logger
{
    internal class GoogleSheetIdeaLogger : IIdeaLogger
    {
        public Task LogIdeaAsync(Idea idea, WordPressUser wordPressUser, UserPrincipal adUser)
        {
            
        }
    }
}
