using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// People who have some sort of stake in ideas.
    /// </summary>
    public class Stakeholder : EntityBase
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

        /// <summary>
        /// The type of stakeholder: owner, etc.
        /// </summary>
        [Required]
      
        public string Type { get; set; }

    }
}