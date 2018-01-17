using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Integration.Logger
{
    public static class IdeaLoggerFactory
    {
        public static IIdeaLogger CreateGoogleSheetIdeaLogger(string serviceAccountPrivateKey,
            string serviceAccountEmail,
            string spreadsheetId,
            string ideasApiBaseUrl)
        {
            return new GoogleSheetIdeaLogger(serviceAccountPrivateKey, serviceAccountEmail, spreadsheetId, ideasApiBaseUrl);
        }
    }
}
