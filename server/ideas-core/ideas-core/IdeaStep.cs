using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core
{
    public class IdeaStep
    {
        public DateTimeOffset CompletedDate { get; set; }
        public object Data { get; set; }
        public string Name { get; set; }
        public int Step { get; set; }
        public string Type { get; set; }
    }
}
