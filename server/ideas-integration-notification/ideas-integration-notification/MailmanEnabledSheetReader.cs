using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    internal class MailmanEnabledSheetReader : IMailmanEnabledSheetReader
    {
        public MailmanEnabledSheetReader(string serviceAccountPrivateKey,
            string serviceAccountEmail,
            string spreadsheetId)
        {
            if (string.IsNullOrWhiteSpace(serviceAccountPrivateKey))
                throw new ArgumentNullException("serviceAccountPrivateKey");
            if (string.IsNullOrWhiteSpace(serviceAccountEmail))
                throw new ArgumentNullException("serviceAccountEmail");
            if (string.IsNullOrWhiteSpace(spreadsheetId))
                throw new ArgumentNullException("spreadsheetid");

            _spreadsheetId = spreadsheetId;

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
                SheetsService.Scope.SpreadsheetsReadonly
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
                            ApplicationName = "Octavia Notification"
                        }
                    );
                }
                return sheetsService;
            }
        }


        public async Task<dynamic> GetMergeTemplateAsync(string templateName)
        {
            string configSheetName = "mm-config";

            // get at most 128 rows, that should be plenty
            var valuesRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, $"{ configSheetName }!B1:B128");
            var valuesResponse = await valuesRequest.ExecuteAsync();

            foreach (var row in valuesResponse.Values)
            {
                // should only be at most object in each row
                var valueObj = row.FirstOrDefault();
                if (valueObj != null)
                {
                    var value = JObject.Parse(valueObj.ToString());
                    var mergeData = value?["mergeData"];
                    var title = value?["title"]?.ToString();
                    if (templateName.Equals(title, StringComparison.OrdinalIgnoreCase))
                    {
                        return mergeData;
                    }
                }
            }

            return null;
        }

        public Task<IDictionary<string, object>> GetValuesAsync(long ideaId)
        {
            // TODO: MergeTemplate definitions should be cached because they change infrequently

            throw new NotImplementedException();
        }
    }
}