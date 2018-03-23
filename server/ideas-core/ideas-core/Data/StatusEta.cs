using CoE.Ideas.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    internal class StatusEta : Entity<int>
    {
        public EtaType EtaType { get; set; }
        // note the largest value here is +- 2 billion, about 63 years if set to BusinessSeconds
        public int Time { get; set; }
        public InitiativeStatus Status { get; set; }
    }

    internal enum EtaType
    {        
        BusinessDays = 1,
        BusinessSeconds = 2
    }
}
