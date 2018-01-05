using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.WordPress
{
    public interface IWordPressClient
    {
        Task<WordPressUser> GetCurrentUserAsync();

        // TODO: create a new WordPress Post Type and 
        // create those Posts when creating ideas.
        //Task<WordPressPost> PostIdea(Idea idea);
    }
}
