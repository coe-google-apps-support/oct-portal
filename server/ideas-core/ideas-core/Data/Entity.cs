using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    // based on "Domain-Driven Design Fundamentals" PluralSight course by Julie Lerman and Steve Smith
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        {
            if (object.Equals(id, default(TId)))
            {
                throw new ArgumentException("The ID Cannot be the type's default value.", "id");
            }

            this.Id = id;
        }

        // EF requires an empty constructor
        protected Entity() { }

        private IList<INotification> _domainEvents;
        public IList<INotification> DomainEvents => _domainEvents;

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            if (_domainEvents is null) return;
            _domainEvents.Remove(eventItem);
        }

        /// <summary>
        /// Sets/Returns the identity of the person or process that created the item.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identity contained within this property is dependent upon the authentication method used by the application.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [StringLength(128)]
        [MaxLength(128)]
        [Display(Description = "Contains the identity of the person or process that created the item.", Name = "Created By")]
        public string AuditCreatedBy { get; private set; }

        /// <summary>
        /// Sets/Returns the date and time that the item was created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The date and time value is in the <see cref="DateTimeOffset"/> format.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [Display(Description = "Contains the creation date and time for the item.", Name = "Created On")]
        public DateTimeOffset? AuditCreatedOn { get; private set; }

        /// <summary>
        /// Sets/Returns the identity of the person or process that last updated the item.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identity contained within this property is dependent upon the authentication method used by the application.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [StringLength(128)]
        [MaxLength(128)]
        [Display(Description = "Contains the identity of the person or process that last updated the item.", Name = "Updated By")]
        public string AuditUpdatedBy { get; protected set; }

        /// <summary>
        /// Sets/Returns the date and time that the item was last updated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The date and time value is in the <see cref="DateTimeOffset"/> format.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [Display(Description = "Contains the date and time when the item was last updated.", Name = "Updated On")]
        public DateTimeOffset? AuditUpdatedOn { get; private set; }

        // For simple entities, this may suffice
        // As Evans notes earlier in the course, equality of Entities is frequently not a simple operator
        public override bool Equals(object otherObject)
        {
            var entity = otherObject as Entity<TId>;
            if (entity != null)
            {
                return this.Equals(entity);
            }
            return base.Equals(otherObject);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Entity<TId> other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }
    }
}
