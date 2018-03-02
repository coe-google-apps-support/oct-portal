using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    /// <summary>
    /// The progression of initiatives through different statuses
    /// </summary>
    [Table("IdeaStatusHistories")]
    internal class IdeaStatusHistoryInternal : EntityBaseInternal
    {
        /// <summary>
        /// The Initiative
        /// </summary>
        public IdeaInternal Initiative { get; set; }

        /// <summary>
        /// The status of the initiatve
        /// </summary>
        public InitiativeStatusInternal Status { get; set; }

        /// <summary>
        /// The Date the initiative entered into the Status
        /// </summary>
        public DateTime StatusEntryDateUtc { get; set; }

        /// <summary>
        /// Message describing the status of the initiative. 
        /// It should be present tense if the initiative is currently in this
        /// status, or it can be updated to a past tense message if this
        /// record is not the current record.
        /// </summary>
        [Display(Name = "Text", Description = "Status description")]
        public string Text { get; set; }

        public virtual PersonInternal Assignee { get; set; }
    }
}
