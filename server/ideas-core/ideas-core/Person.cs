using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core
{
    public class Person : EntityBase
    {
        /// <summary>
        /// The display name of the stakeholder.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The email address of the stakeholder.
        /// </summary>
        [Required]
        public string Email { get; set; }

    }
}
