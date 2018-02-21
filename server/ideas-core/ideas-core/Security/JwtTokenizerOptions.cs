using System;

namespace CoE.Ideas.Core.Security
{
    public class JwtTokenizerOptions
    {
        public Uri WordPressUrl { get; set; }
        public string JwtSecretKey { get; set; }
    }
}