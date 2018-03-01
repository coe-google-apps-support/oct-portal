using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    [Table("StringTemplates")]
    internal class StringTemplateInternal : EntityBaseInternal
    {
        public string Text { get; set; }
        public StringTemplateCategory Category { get; set; }
        public string Key { get; set; }
    }
}
