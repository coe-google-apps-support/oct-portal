using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.EndToEnd.Tests.Mocks
{
    internal class RemedyWorkOrder
    {
        public string WorkOrderId { get; set; }
        public Initiative Idea { get; set; }
    }
}
