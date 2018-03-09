using CoE.Ideas.Core.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.WordPress
{
    public interface IWordPressClient
    {
        ClaimsPrincipal User { get; set; }

        Task<WordPressUser> GetCurrentUserAsync();

        Task<WordPressUser> GetUserAsync(int wordPressuserId);

        Task<WordPressPost> PostIdeaAsync(Initiative idea);

        Task<WordPressPost> GetPostForInitativeSlug(string slug);
    }
}
