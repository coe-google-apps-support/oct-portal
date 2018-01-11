using Newtonsoft.Json;

namespace CoE.Ideas.Core.WordPress
{
    public class WordPressPost
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        // more fields can be added here
        // for a list go here: https://developer.wordpress.org/rest-api/reference/posts/#schema
    }
}