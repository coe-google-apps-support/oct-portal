using CoE.Issues.Core.Remedy;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Remedy.Watcher
{
    public class RemedyPollResult
    {
        public RemedyPollResult()
        {
            RecordsProcesed = new List<Incident>();
            ProcessErrors = new List<ProcessError>();
            StartTimeUtc = DateTime.Now.ToUniversalTime();
        }

        public RemedyPollResult(DateTime startLastModifiedTimeUtc) : this()
        {
            StartLastModifiedTimeUtc = startLastModifiedTimeUtc;
            EndTimeUtc = startLastModifiedTimeUtc; // default value; should be overwritten by calling classes
        }

        public DateTime StartTimeUtc { get; set; }
        public DateTime StartLastModifiedTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public ICollection<Incident> RecordsProcesed { get; set; }
        public ICollection<ProcessError> ProcessErrors { get; set; }
    }
}
