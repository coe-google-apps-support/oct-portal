using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Logger
{
    public interface IIdeaLogger
    {
        // I know it's bad to return a Google specific resturn value, but it's really convenient 
        // and I'm in a hurry -DC
        Task<AppendValuesResponse> LogIdeaAsync(Idea idea, WordPressUser wordPressUser, UserPrincipal adUser);
    }
}
