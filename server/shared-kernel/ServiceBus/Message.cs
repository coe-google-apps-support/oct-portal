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
        public IDictionary<string, object> MessageProperties { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string LockToken { get; set; }
        public object Value { get; set; }
    }

}
