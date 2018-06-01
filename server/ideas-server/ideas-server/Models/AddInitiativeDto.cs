using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Models
{
    public class AddInitiativeDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public SupportingDocumentsDto[] supportingDocumentsDto { get; set; }


    }
}
