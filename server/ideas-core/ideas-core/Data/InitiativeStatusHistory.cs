using CoE.Ideas.Shared.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    public class InitiativeStatusHistory : Entity<int>
    {

        // EF requires an empty constructor
        protected InitiativeStatusHistory() { }

        /// <summary>
        /// The status of the initiatve
        /// </summary>
        public InitiativeStatus Status { get; private set; }

        /// <summary>
        /// The Date the initiative entered into the Status, in Universal time
        /// </summary>
        public DateTime StatusEntryDateUtc { get; private set; }


        /// <summary>
        /// Message describing the status of the initiative. 
        /// It should be present tense if the initiative is currently in this
        /// status, or it can be updated to a past tense message if this
        /// record is not the current record.
        /// </summary>
        [MaxLength(1024)]
        public string Text { get; private set; }

        // foreign key to Initiatives
        public Guid InitiativeId { get; private set; }

        /// <summary>
        /// The id of the person assigned to the initiative
        /// </summary>
        public int? PersonId { get; private set; }


        internal static InitiativeStatusHistory CreateInitiativeStatusChange(Guid initiativeId,
            InitiativeStatus newStatus,
            DateTime statusEntryDateUtc,
            int? personId,
            string text)
        {
            return new InitiativeStatusHistory()
            {
                InitiativeId = initiativeId,
                Status = newStatus,
                StatusEntryDateUtc = statusEntryDateUtc,
                PersonId = personId,
                Text = text
            };
        }

        internal void UpdateText(string newText)
        {
            Text = newText;
        }

    }
}
