using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.WordPress
{
    public class WordPressUser
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Login name for the user.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Display name for the user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// First name for the user.
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name for the user.
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }


        /// <summary>
        /// The email address for the user.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// URL of the user.
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Description of the user.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Author URL of the user.
        /// </summary>
        [JsonProperty("link")]
        public Uri Link { get; set; }

        /// <summary>
        /// Locale for the user.
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// The nickname for the user.
        /// </summary>
        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// An alphanumeric identifier for the user.
        /// </summary>
        [JsonProperty("slug")]
        public string Slug { get; set; }

        ///// <summary>
        ///// Registration date for the user.
        ///// </summary>
        //[JsonProperty("registered_date")]
        //public DateTime RegisteredDate { get; internal set; }

        /// <summary>
        /// Roles assigned to the user.
        /// </summary>
        [JsonProperty("roles")]
        public string[] Roles { get; set; }

    }
}
