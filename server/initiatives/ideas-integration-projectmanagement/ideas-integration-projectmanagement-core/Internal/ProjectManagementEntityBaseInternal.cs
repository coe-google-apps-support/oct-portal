using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    /// <summary>
    /// Base class for all Entities
    /// </summary>
    internal abstract class ProjectManagementEntityBaseInternal
    {
        /// <summary>
        /// Identifier for the Entity
        /// </summary>
        public long Id { get; set; }
    }
}
