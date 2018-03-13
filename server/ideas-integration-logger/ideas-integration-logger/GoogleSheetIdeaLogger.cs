using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.WordPress;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;

namespace CoE.Ideas.Integration.Logger
{
    internal class GoogleSheetIdeaLogger : IIdeaLogger
    {
        public GoogleSheetIdeaLogger(string serviceAccountPrivateKey,
            string serviceAccountEmail,
            string spreadsheetId,
            string ideasApiBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceAccountPrivateKey))
                throw new ArgumentNullException("serviceAccountPrivateKey");
            if (string.IsNullOrWhiteSpace(serviceAccountEmail))
                throw new ArgumentNullException("serviceAccountEmail");
            if (string.IsNullOrWhiteSpace(spreadsheetId))
                throw new ArgumentNullException("spreadsheetid");

            _spreadsheetId = spreadsheetId;
            _ideasApiBaseUrl = ideasApiBaseUrl;

            if (!string.Equals(_serviceAccountPrivateKey, serviceAccountPrivateKey, StringComparison.Ordinal) ||
                !string.Equals(_serviceAccountEmail, serviceAccountEmail, StringComparison.Ordinal))
            {
                credential = null;
                _serviceAccountPrivateKey = serviceAccountPrivateKey;
                _serviceAccountEmail = serviceAccountEmail;
            }
        }

        private static string _serviceAccountPrivateKey;
        private static string _serviceAccountEmail;
        private readonly string _ideasApiBaseUrl;
        private readonly string _spreadsheetId;


        private static ICredential credential;
        protected ICredential Credentials
        {
            get
            {
                if (credential == null)
                {
                    // Create an explicit ServiceAccountCredential credential
                    credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(_serviceAccountEmail)
                    {
                        Scopes = GetCredentialScopes()
                    }.FromPrivateKey(_serviceAccountPrivateKey));
                }

                return credential;
            }
        }

        protected virtual string[] GetCredentialScopes()
        {
            return new[] {
                    SheetsService.Scope.Spreadsheets
                };
        }

        private SheetsService sheetsService;
        protected virtual SheetsService SheetsService
        {
            get
            {
                if (sheetsService == null)
                {
                    sheetsService = new SheetsService(
                        new Google.Apis.Services.BaseClientService.Initializer()
                        {
                            HttpClientInitializer = Credentials,
                            ApplicationName = "Octavia Logger"
                        }
                    );
                }
                return sheetsService;
            }
        }


        public async Task<AppendValuesResponse> LogIdeaAsync(Initiative idea, ClaimsPrincipal owner, UserPrincipal adUser)
        {
            var values = new ValueRange() { MajorDimension = "ROWS" };
            IList<object> rowData = new List<object>()
            {
                idea.Id,
                idea.Title,
                idea.Description,
                idea.Url,
                $"{_ideasApiBaseUrl}/{idea.Id}",
                owner.GetDisplayName(),
                owner.GetEmail(),
                adUser?.SamAccountName,
                idea.CreatedDate
            };

            values.Values = new List<IList<object>> { rowData };

            var range = "Initiatives!A1:F1";

            var request = SheetsService.Spreadsheets.Values.Append(values, _spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            try
            {
                AppendValuesResponse response = await request.ExecuteAsync();
                return response;
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.TraceError($"Unable to append values to Google sheet: { err.Message}");
                throw;
            }
        }
    }
}
