using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// A City of Edmonton Department within a Branch
    /// </summary>
    public class Department : EntityBase
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
        public bool IsActive { get; set; }

        /// <summary>
        /// The <see cref="Branch"/> that this department belongs to
        /// </summary>
        [Required]
        public Branch Branch { get; set; }

    }
}
