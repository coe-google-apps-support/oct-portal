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
        [Required]
        public PersonInternal Person { get; set; }

        /// <summary>
        /// The type of stakeholder: owner, etc.
        /// </summary>
        [Required]
        [DefaultValue(StakeholderTypeInternal.Owner)]
        public StakeholderTypeInternal Type { get; set; }
    }
}
