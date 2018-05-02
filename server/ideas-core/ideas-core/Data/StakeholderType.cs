using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    /// <summary>
    /// The type of stakeholder: owner, etc.
    /// </summary>
    public enum StakeholderType
    {
        /// <summary>
        /// Person who created the idea
        /// </summary>
        Requestor = 1,

        /// <summary>
        /// The person responsible for the idea when the Owner is different than the BusinessContact
        /// </summary>
        BusinessContact = 2
            
    }
}
