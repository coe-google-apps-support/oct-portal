using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    /// <summary>
    /// People who have some sort of stake in ideas.
    /// </summary>
    [Table("Stakeholders")]
    internal class StakeholderInternal : EntityBaseInternal
    {
        /// <summary>
        /// The display name of the stakeholder.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The email address of the stakeholder.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The type of stakeholder: owner, etc.
        /// </summary>
        [Required]
        [DefaultValue(StakeholderTypeInternal.Owner)]
        public StakeholderTypeInternal Type { get; set; }
    }
}
