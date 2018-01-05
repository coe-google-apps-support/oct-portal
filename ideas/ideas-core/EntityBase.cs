using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// Base class for all Entities
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Identifier for the Entity
        /// </summary>
        public long Id { get; set; }
    }
}
