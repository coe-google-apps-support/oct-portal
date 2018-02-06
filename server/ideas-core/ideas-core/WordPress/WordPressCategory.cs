using Newtonsoft.Json;

namespace CoE.Ideas.Core.WordPress
{
    internal class WordPressCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        // more fields can be added here...
        // for a list go to https://developer.wordpress.org/rest-api/reference/categories/#schema
    }
}