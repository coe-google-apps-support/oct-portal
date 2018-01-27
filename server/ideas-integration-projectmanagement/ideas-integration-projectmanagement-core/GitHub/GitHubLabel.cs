using CoE.Ideas.Core.ProjectManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.GitHub
{
    public class GitHubLabel : ProjectManagementEntityBase
    {
        [JsonProperty("label_id")]
        public override long Id { get => base.Id; set => base.Id = value; }

        [JsonProperty("id")]
        public string AlternateKey { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("default")]
        public bool IsDefault { get; set; }
    }
}
