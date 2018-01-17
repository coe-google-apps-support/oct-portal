using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Logger
{
    public interface IIdeaLogger
    {
        Task LogIdeaAsync(Idea idea, WordPressUser wordPressUser, UserPrincipal adUser);
    }
}
