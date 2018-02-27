using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.WordPress
{
    public interface IWordPressClient
    {
        ClaimsPrincipal User { get; set; }

        Task<WordPressUser> GetCurrentUserAsync();

        Task<WordPressUser> GetUserAsync(int wordPressuserId);

        Task<WordPressPost> PostIdeaAsync(Idea idea);

        Task<WordPressPost> GetPostForInitativeSlug(string slug);
    }
}
