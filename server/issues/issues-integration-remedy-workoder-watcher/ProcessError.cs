using CoE.Issues.Remedy.WorkOrder.Watcher.RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Remedy.WorkOrder.Watcher
{
    public class ProcessError
    {
        public OutputMapping1GetListValues WorkItem { get; set;}
        public string ErrorMessage { get; set; }
    }
}
