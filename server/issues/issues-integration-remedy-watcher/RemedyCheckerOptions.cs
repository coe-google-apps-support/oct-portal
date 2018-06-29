using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Remedy.Watcher
{
    public class RemedyCheckerOptions
    {
        public string ServiceUserName { get; set; }
        public string ServicePassword { get; set; }
        public string TemplateName { get; set; }
        public string ApiUrl { get; set; }
        public string TempDirectory { get; set; }
    }
}
