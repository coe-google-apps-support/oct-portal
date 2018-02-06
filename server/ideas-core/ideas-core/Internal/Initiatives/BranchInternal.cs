using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    [Table("Branches")]
    internal class BranchInternal : EntityBaseInternal
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

    }
}
