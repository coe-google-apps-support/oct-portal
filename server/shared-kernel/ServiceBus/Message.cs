using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CoE.Ideas.Shared.ServiceBus
{
    public class Message
    {
        public Guid Id { get; set; }
        [MaxLength(128)]
        public string Label { get; set; }
        [MaxLength(2048)]
        public string Text { get; set; }
        public IDictionary<string, object> MessageProperties { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
