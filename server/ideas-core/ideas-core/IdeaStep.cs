using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core
{
    public class IdeaStep
    {
        public string Title { get; set; }
        public string Description { get; set; }
        
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
    }
}
