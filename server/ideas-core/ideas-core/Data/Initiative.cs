using CoE.Ideas.Core.Events;
using CoE.Ideas.Shared.Data;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Data
{
    // Initiative is a Domain Driven Design aggregate root 
    public class Initiative : AggregateRoot<int>
    {
        public Initiative(Guid uid) : base()
        {
            Uid = uid;
            _statusHistories = new HashSet<InitiativeStatusHistory>();
        }

        private Initiative() : base() { } // required for EF

        // Factory method for creation
        public static Initiative Create(
            string title, 
            string description,
            int ownerPersonId,
            bool skipEmailNotification = false)
        {
            Ensure.String.IsNotNullOrWhiteSpace(title, nameof(title));
            Ensure.String.IsNotNullOrWhiteSpace(description, nameof(description));
            Ensure.Comparable.IsGt(ownerPersonId, 0, nameof(ownerPersonId));

            var initiative = new Initiative(Guid.NewGuid());
            initiative.Title = title;
            initiative.Description = description;
            initiative.Stakeholders = new List<Stakeholder>()
            {
                Stakeholder.Create(ownerPersonId, StakeholderType.Owner)
            };
            initiative.Status = InitiativeStatus.Initiate;
            initiative.StatusHistories = new HashSet<InitiativeStatusHistory>();
            initiative.CreatedDate = DateTimeOffset.Now;

            initiative.AddDomainEvent(new InitiativeCreatedDomainEvent(initiative.Uid, ownerPersonId, skipEmailNotification));

            return initiative;
        }


        public Guid Uid { get; private set; }

        /// <summary>
        /// The short title of the idea
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Title { get; private set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        [Required]
        public string Description { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }


        // best practice is to have only one-way navigation properties, where possible
        /// <summary>
        /// The people that have some stake in the idea, will always include the owner
        /// </summary>
        public ICollection<Stakeholder> Stakeholders { get; private set; }

        /// <summary>
        /// The person currently assigned to the initiative, usually a Business Analyst
        /// </summary>
        /// <remarks>
        /// Can be null
        /// </remarks>
        public int? AssigneeId { get; private set; }


        /// <summary>
        /// Unique identifier in Work Order Tracking system (Remedy)
        /// </summary>
        [MaxLength(128)]
        public string WorkOrderId { get; private set; }

        /// <summary>
        /// Status of the Initiative
        /// </summary>
        public InitiativeStatus Status { get; private set; }

        internal ICollection<InitiativeStatusHistory> _statusHistories;
        public IEnumerable<InitiativeStatusHistory> StatusHistories
        {
            get { return _statusHistories; }
            private set
            {
                if (value == null)
                    _statusHistories = new HashSet<InitiativeStatusHistory>();
                else
                {
                    var theValue = value as ICollection<InitiativeStatusHistory>;
                    if (theValue == null)
                        _statusHistories = value.ToList();
                    else
                        _statusHistories = theValue;
                }
            }
        }

        /// <summary>
        /// Business case for the initiative
        /// </summary>
        [Display(Name = "Business Case URL", Description = "The location of the businses case for the initiative")]
        [MaxLength(2048)]
        public string BusinessCaseUrl { get; private set; }

        public void SetBusinessCaseUrl(string newBusinsesCaseUrl)
        {
            BusinessCaseUrl = newBusinsesCaseUrl;
            //AddDomainEvent(new BusinessCaseUrlChangedDomainEvent(Uid, newBusinsesCaseUrl));
        }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Investment Request Form Url", Description = "The link the to associated Investment Request Form")]
        [MaxLength(2048)]
        public string InvestmentRequestFormUrl { get; private set; }

        public void SetInvestmentFormUrl(string newInvestmentRequestFormUrl)
        {
            InvestmentRequestFormUrl = newInvestmentRequestFormUrl;
            //AddDomainEvent(new BusinessCaseUrlChangedDomainEvent(Uid, newBusinsesCaseUrl));
        }

        public void SetWorkOrderId(string newWorkOrderId)
        {
            Ensure.String.IsNotNullOrWhiteSpace(newWorkOrderId, nameof(newWorkOrderId));

            WorkOrderId = newWorkOrderId;           

            #region InitiativeUpdated Event

            #endregion
        }

        public void SetAssigneeId(int? assigneeId)
        {
            AssigneeId = assigneeId;

            #region InitiativeUpdated Event
            #endregion
        }

        public void UpdateStatus(InitiativeStatus newStatus, 
            DateTime? newEta)
        {
            var oldStatus = Status;
            Status = newStatus;

            // pop any stathistories after or including this one
            foreach (var toRemove in _statusHistories.Where(x => (int)x.Status > (int)newStatus).ToList())
            {
                _statusHistories.Remove(toRemove);
            }
            var existingStatus = _statusHistories.SingleOrDefault(x => x.Status == newStatus);

            if (existingStatus == null)
            {
                _statusHistories.Add(
                    InitiativeStatusHistory.CreateInitiativeStatusChange(
                        Uid, newStatus, DateTime.UtcNow, newEta, AssigneeId));
            }
            else
            {
                if (existingStatus.PersonId != AssigneeId)
                    existingStatus.SetPersonId(AssigneeId);
            }

            AddDomainEvent(new InitiativeStatusChangedDomainEvent(this, oldStatus));
        }

        public void UpdateCurrentStatusDescription(string newDescription)
        {
            var currentStatusHistory = this._statusHistories
                .OrderByDescending(x => x.StatusEntryDateUtc)
                .FirstOrDefault();

            if (currentStatusHistory == null)
                throw new InvalidOperationException("There is no status history for this initiative");

            if (string.IsNullOrWhiteSpace(newDescription))
                currentStatusHistory.ResetStatusDescriptionToDefault();
            else
                currentStatusHistory.OverrideStatusDescription(newDescription);

            AddDomainEvent(new InitiativeStatusDescriptionUpdatedDomainEvent(this, newDescription));
        }
    }
}
