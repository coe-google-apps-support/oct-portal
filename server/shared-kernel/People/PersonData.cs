using Newtonsoft.Json;
using System;

namespace CoE.Ideas.Shared.People
{
    public class PersonData
    {
        public Guid Id { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        [JsonProperty("Mail")]
        public string Email { get; set; }

        [JsonProperty("NetworkId")]
        public string User3and3 { get; set; }

        public string Telephone { get; set; }

        public string Title { get; set; }

        public string DisplayName => $"{GivenName} {Surname}";
    }
}
