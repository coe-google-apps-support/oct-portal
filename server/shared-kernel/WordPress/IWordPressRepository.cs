using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.WordPress
{
    public interface IWordPressRepository
    {
        Task<WordPressUser> GetUserAsync(int id);
        Task<WordPressUser> GetUserByEmailAsync(string email);
    }
}
