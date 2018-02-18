using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy
{
    public class RemedyServiceOptions
    {
        public string ServiceUserName { get; set; }
        public string ServicePassword { get; set; }

        public string CustomerCompany { get; set; }
        public string CustomerLoginId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string LocationCompany { get; set; }
        public string WorkOrderTemplateUsed { get; set; }
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
        public string CategorizationTier1 { get; set; }
        public string CategorizationTier2 { get; set; }
        public string CategorizationTier3 { get; set; }
    }
}
