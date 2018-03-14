using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using Google.Apis.Sheets.v4.Data;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Logger
{
    public interface IIdeaLogger
    {
        // I know it's bad to return a Google specific resturn value, but it's really convenient 
        // and I'm in a hurry -DC
        Task<AppendValuesResponse> LogIdeaAsync(Initiative idea, ClaimsPrincipal owner, UserPrincipal adUser);
    }
}
