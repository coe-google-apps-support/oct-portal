using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Shared.ServiceBus
{
    internal class MessageProperty
    {
        public int Id { get; set; }
        [MaxLength(128)]
        public string Key { get; set; }
        [MaxLength(1024)]
        public string Value { get; set; }
        [MaxLength(1024)]
        public string ValueType { get; set; }
        public Guid MessageId { get; set; }
    }
}
