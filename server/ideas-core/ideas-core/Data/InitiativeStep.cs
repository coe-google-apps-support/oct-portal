using System;

namespace CoE.Ideas.Core.Data
{
    public class InitiativeStep
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
    }
}
