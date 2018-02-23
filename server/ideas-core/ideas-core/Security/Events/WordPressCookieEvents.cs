using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Security.Events
{
    public class WordPressCookieEvents
    {
        public WordPressCookieEvents()
        {
        }

        public Func<CookieReceivedContext, Task> OnValidateCookie { get; set; }

    }
}
