using CoE.Ideas.Shared.Data;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    internal class InitiativeStatusHistory : Entity<int>
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
        /// The date and time the initiative is expected to exit the current status,
        /// if known.
        /// </summary>
        public DateTime? ExpectedExitDateUtc { get; private set; }

        // foreign key to Initiatives
        public Guid InitiativeId { get; private set; }

        /// <summary>
        /// The id of the person assigned to the initiative
        /// </summary>
        public int? PersonId { get; private set; }

        /// <summary>
        /// When supplied, overrides the default string templates for the status descriptions
        /// </summary>
        public string StatusDescriptionOverride { get; private set; }

        internal void OverrideStatusDescription(string newStatusDescription)
        {
            EnsureArg.IsNotNullOrWhiteSpace(newStatusDescription);
            StatusDescriptionOverride = newStatusDescription;
        }

        internal void ResetStatusDescriptionToDefault()
        {
            StatusDescriptionOverride = null;
        }

        internal static InitiativeStatusHistory CreateInitiativeStatusChange(Guid initiativeId,
            InitiativeStatus newStatus,
            DateTime statusEntryDateUtc,
            DateTime? expectedExitDateUtc,
            int? personId)
        {
            return new InitiativeStatusHistory()
            {
                InitiativeId = initiativeId,
                Status = newStatus,
                StatusEntryDateUtc = statusEntryDateUtc,
                ExpectedExitDateUtc = expectedExitDateUtc,
                PersonId = personId
            };
        }

    }
}
