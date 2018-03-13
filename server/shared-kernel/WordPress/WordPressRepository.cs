using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.WordPress
{
    internal class WordPressRepository : IWordPressRepository
    {
        public WordPressRepository(WordPressContext context)
        {
            EnsureArg.IsNotNull(context);
            _context = context;
        }

        private readonly WordPressContext _context;

        public async Task<WordPressUser> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return null;

            // we should use AutoMapper for this.
            return new WordPressUser()
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Url = user.Url
            };
        }

        public async Task<WordPressUser> GetUserByEmailAsync(string email)
        {
            // try local first
            var user = _context.Users.Local.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                // now try remote
                user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            }


            if (user == null)
                return null;

            // we should use AutoMapper for this.
            return new WordPressUser()
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Url = user.Url
            };
        }
    }
}
