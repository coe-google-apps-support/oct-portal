using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Integration.Logger;
using Google.Apis.Sheets.v4.Data;

namespace CoE.Ideas.EndToEnd.Tests.Mocks
{
    internal class MockIdeaLogger : IIdeaLogger
    {
        public MockIdeaLogger()
        {
            initiativesLogged = new List<Initiative>();
        }

        private readonly ICollection<Initiative> initiativesLogged;
        public IEnumerable<Initiative> InitiativesLogged { get { return initiativesLogged; } }


        public Task<AppendValuesResponse> LogIdeaAsync(Initiative idea, ClaimsPrincipal owner, UserPrincipal adUser)
        {
            initiativesLogged.Add(idea);

            // mock data
            AppendValuesResponse returnValue = new AppendValuesResponse
            {
                TableRange = "$A1:Z1",
                SpreadsheetId = "TestSpreadsheet"
            };

            return Task.FromResult(returnValue);
        }
    }
}