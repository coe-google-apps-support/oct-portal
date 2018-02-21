using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    [Table("People")]
    internal class PersonInternal : EntityBaseInternal
    {
        /// <summary>
        /// The display name of the stakeholder.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        /// <summary>
        /// The email address of the stakeholder.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }


    }
}
