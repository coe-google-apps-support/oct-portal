using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.WordPress
{
    public class WordPressUser
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Login name for the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Display name for the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email address for the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// URL of the user.
        /// </summary>
        public string Url { get; set; }
    }
}
