using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// The progression of initiatives through different statuses
    /// </summary>
    public class IdeasStatusHistory : EntityBase
    {
        /// <summary>
        /// The Initiative
        /// </summary>
        public Idea Initiative { get; set; }

        /// <summary>
        /// The status of the initiatve
        /// </summary>
        public InitiativeStatus Status { get; set; }

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

        public Person Assignee { get; set; }

    }
}
