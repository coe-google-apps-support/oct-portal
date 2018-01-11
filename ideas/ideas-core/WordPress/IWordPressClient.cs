using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.WordPress
{
    public interface IWordPressClient
    {
        Task<WordPressUser> GetCurrentUserAsync();

        
        Task<WordPressPost> PostIdeaAsync(Idea idea);
    }
}
