using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    public class InitiativeInfo
    {
        public int Id { get; private set; }

        /// <summary>
        /// The short title of the idea
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Title { get; private set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        [Required]
        [MinLength(3)]
        public string Description { get; private set; }

        [Required]
        public DateTimeOffset CreatedDate { get; private set; }

        internal static InitiativeInfo Create(Initiative initiative)
        {
            return new InitiativeInfo()
            {
                Id = initiative.Id,
                Title = initiative.Title,
                Description = initiative.Description,
                CreatedDate = initiative.CreatedDate
            };
        }
    }
}
