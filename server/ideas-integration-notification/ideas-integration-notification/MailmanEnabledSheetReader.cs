using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    var title = mergeData?["title"]?.ToString();
                    if (templateName.Equals(title, StringComparison.OrdinalIgnoreCase))
                    {
                        return mergeData;
                    }
                }
            }

            return null;
        }

        public Task<IDictionary<string, object>> GetValuesAsync(dynamic mergeTemplate, long ideaId)
        {
            // TODO: MergeTemplate definitions should be cached because they change infrequently

            throw new NotImplementedException();
        }

        public async Task<IDictionary<string, object>> GetValuesAsync(dynamic mergeTemplate, string range)
        {
            if (mergeTemplate == null)
                throw new ArgumentNullException("mergeTemplate");
            if (string.IsNullOrWhiteSpace(range))
                throw new ArgumentNullException("range");

            // this is the best I could come up with on short notice to get the columns/rows in the given range
            string rangeSimple = range, sheetName = string.Empty;
            if (rangeSimple.IndexOf("!") >= 0)
            {
                int i = rangeSimple.IndexOf("!");
                if (rangeSimple.Length > i + 1)
                {
                    sheetName = i > 0 ? rangeSimple.Substring(0, i) : string.Empty;
                    rangeSimple = rangeSimple.Substring(i + 1);
                }
            }

            var m = Regex.Match(rangeSimple, @"^([A-Z]+)(\d+):([A-Z]+)(\d+)$");
            if (!m.Success || m.Groups?.Count < 4)
                throw new ArgumentOutOfRangeException("Expecting range to be in the format A1:B2 format");


            // get the last row in the range for value
            var valuesRowIndex = int.Parse(m.Groups[4].Value) + 1;
            string valuesRange =
                (!string.IsNullOrWhiteSpace(sheetName) ? $"{sheetName}!" : string.Empty) + 
                m.Groups[1] + valuesRowIndex + ":" + m.Groups[3] + valuesRowIndex;

            // header range is row defined in mergeTemplate and columns defined in range
            var headerRange =
                (!string.IsNullOrWhiteSpace(sheetName) ? $"{sheetName}!" : string.Empty) + 
                m.Groups[1] + mergeTemplate.headerRow + ":" + m.Groups[3] + mergeTemplate.headerRow;

            var batchGet = SheetsService.Spreadsheets.Values.BatchGet(_spreadsheetId);
            batchGet.Ranges = new Google.Apis.Util.Repeatable<string>(new string[] { headerRange, valuesRange });


            IList<object> headerRow;
            IList<object> valuesRow;
            try
            {
                var result = await batchGet.ExecuteAsync();
                headerRow = result.ValueRanges[0].Values.FirstOrDefault();
                valuesRow = result.ValueRanges[1].Values.FirstOrDefault();
            }
            catch (Exception err)
            {
                Trace.TraceError($"Error retriving values or header row: { err.Message }");
                throw;
            }

            if (valuesRow == null)
                throw new InvalidOperationException("No data returned from Google Sheet");
            if (headerRow == null)
                throw new InvalidOperationException("No header row returned from Google Sheet");

            int index = Math.Min(valuesRow.Count, headerRow.Count);

            var returnValue = new Dictionary<string, object>();
            for (int j=0; j<index; j++)
            {
                string s = headerRow[j] as string;
                if (!string.IsNullOrWhiteSpace(s))
                    returnValue[s] = valuesRow[j];
            }

            return returnValue;
        }
    }
}