using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Data
{
    public interface IContainsDomainEvents
    {
        IEnumerable<INotification> DomainEvents { get; }

        /// <summary>
        /// Removes the items in the DomainEvents collection. 
        /// (i.e. after they've been handled so they aren't fired multiple times)
        /// </summary>
        void ClearDomainEvents();
    }
}
