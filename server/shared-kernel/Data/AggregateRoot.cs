using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Data
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IContainsDomainEvents
    {
        protected AggregateRoot(TId id) : base(id) { }
        protected AggregateRoot() : base() { }

        private IList<INotification> _domainEvents;
        public IEnumerable<INotification> DomainEvents
        {
            get
            {
                if (_domainEvents == null)
                    _domainEvents = new List<INotification>();
                return _domainEvents;
            }
        }

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

        void IContainsDomainEvents.ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
