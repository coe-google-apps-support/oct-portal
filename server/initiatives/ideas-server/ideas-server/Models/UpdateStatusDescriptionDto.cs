﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Models
{
    public class UpdateStatusDescriptionDto
    {
        public int StepId { get; set; }

        public string NewDescription { get; set; }
    }
}
