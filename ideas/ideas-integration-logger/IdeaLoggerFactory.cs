using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Integration.Logger
{
    public static class IdeaLoggerFactory
    {
        public static IIdeaLogger CreateGoogleSheetIdeaLogger()
        {
            return new GoogleSheetIdeaLogger();
        }
    }
}
