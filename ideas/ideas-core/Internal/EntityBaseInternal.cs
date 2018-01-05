using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Internal
{
    /// <summary>
    /// Base class for all Entities
    /// </summary>
    internal abstract class EntityBaseInternal
    {
        /// <summary>
        /// Identifier for the Entity
        /// </summary>
        public long Id { get; set; }
    }
}
