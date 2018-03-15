using CoE.Ideas.Shared.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    internal class StringTemplate : Entity<int>
    {
        [MaxLength(2048)]
        public string Text { get; set; }
        public StringTemplateCategory Category { get; set; }
        [MaxLength(64)]
        public string Key { get; set; }
    }
}
