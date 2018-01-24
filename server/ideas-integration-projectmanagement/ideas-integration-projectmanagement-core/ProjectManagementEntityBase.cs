using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.ProjectManagement.Core
{
    /// <summary>
    /// Base class for all Entities
    /// </summary>
    public class ProjectManagementEntityBase
    {
        /// <summary>
        /// Identifier for the Entity
        /// </summary>
        public virtual long Id { get; set; }
    }
}
