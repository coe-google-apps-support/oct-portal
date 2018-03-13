using CoE.Ideas.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    internal class StringTemplate : Entity<int>
    {
        public string Text { get; set; }
        public StringTemplateCategory Category { get; set; }
        public string Key { get; set; }
    }
}
