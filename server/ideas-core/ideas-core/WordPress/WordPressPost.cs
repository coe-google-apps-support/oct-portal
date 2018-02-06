using Newtonsoft.Json;
using System;

namespace CoE.Ideas.Core.WordPress
{
    public class WordPressPost
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// URL to the object.
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// The date the object was published, in the site's timezone.
        /// </summary>
        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// The date the object was published, as GMT.
        /// </summary>
        [JsonProperty("date_gmt")]
        public DateTime? DateGmt { get; set; }
        // more fields can be added here
        // for a list go here: https://developer.wordpress.org/rest-api/reference/posts/#schema

        /// <summary>
        /// The date the object was last modified, in the site's timezone.
        /// </summary>
        [JsonProperty("modified")]
        public DateTime? Modified { get; set; }

        /// <summary>
        /// The date the object was last modified, as GMT.
        /// </summary>
        [JsonProperty("modified_gmt")]
        public DateTime? ModifiedGmt { get; set; }

        /// <summary>
        /// An alphanumeric identifier for the object unique to its type.
        /// </summary>
        [JsonProperty("slug")]
        public string Slug { get; set; }

        /// <summary>
        /// A named status for the object.
        /// </summary>
        /// <remarks>
        /// One of: publish, future, draft, pending, private
        /// </remarks>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Type of Post for the object.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The ID for the author of the object.
        /// </summary>
        [JsonProperty("author")]
        public int Author { get; set; }

        /// <summary>
        /// The terms assigned to the object in the category taxonomy.
        /// </summary>
        [JsonProperty("categories")]
        public int[] CategoryIds { get; set; }

        /// <summary>
        /// The terms assigned to the object in the post_tag taxonomy.
        /// </summary>
        [JsonProperty("tags")]
        public int[] TagIds { get; set; }
    }
}