
namespace CoE.Ideas.Shared.WordPress
{
    internal class WordPressUserSecurityOptions
    {
        public string Url { get; set; }

        // naming convention comes from WordPress, do not pascal case
        public string AUTH_KEY { get; set; }
        public string SECURE_AUTH_KEY { get; set; }
        public string LOGGED_IN_KEY { get; set; }
        public string NONCE_KEY { get; set; }
        public string AUTH_SALT { get; set; }
        public string SECURE_AUTH_SALT { get; set; }
        public string LOGGED_IN_SALT { get; set; }
        public string NONCE_SALT { get; set; }
        public string SECRET_KEY { get; set; }
        public string SECRET_SALT { get; set; }

        public bool IS_SSL { get; set; }
    }
}
