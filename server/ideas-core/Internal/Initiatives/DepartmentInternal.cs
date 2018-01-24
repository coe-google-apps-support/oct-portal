using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    /// <summary>
    /// A City of Edmonton Department within a Branch
    /// </summary>
    internal class DepartmentInternal : EntityBaseInternal
    {
        /// <summary>
        /// The name of the branch
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Specifies that the branch is an active branch
        /// </summary>
        [DefaultValue(true)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// The <see cref="BranchInternal"/> that this department belongs to
        /// </summary>
        [Required]
        public BranchInternal Branch { get; set; }

    }
}
