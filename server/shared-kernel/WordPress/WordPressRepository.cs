using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.WordPress
{
    internal class WordPressRepository : IWordPressRepository, IHealthCheckable
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

        Task<IDictionary<string, object>> IHealthCheckable.HealthCheckAsync()
        {
            IDictionary<string, object> returnValue = new Dictionary<string, object>();
            System.Data.Common.DbConnection dbConnection = null;
            try { dbConnection = _context?.Database?.GetDbConnection(); }
            catch (Exception err) { returnValue["dbError"] = err.Message; }

            if (dbConnection != null)
            {
                try { returnValue["host"] = dbConnection.DataSource; } catch (Exception err) { returnValue["host"] = err.Message; }
                try { returnValue["database"] = dbConnection.Database; } catch (Exception err) { returnValue["database"] = err.Message; }

                try
                {
                    _context.Database.OpenConnection();
                    try { returnValue["serverVersion"] = dbConnection.ServerVersion; } catch (Exception err) { returnValue["serverVersion"] = err.Message; }

                    // The following should produce the same as serverVersion, but to be sure we'll run it again.
                    // Also, we'll time it and supply that result

                    using (var cmd = dbConnection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT VERSION();";
                        var watch = new Stopwatch();
                        watch.Start();
                        var result = cmd.ExecuteScalar();
                        watch.Stop();
                        returnValue["version"] = result;
                        returnValue["pingMilliseconds"] = watch.Elapsed.TotalMilliseconds;
                    }
                }
                catch (Exception) { /* eat the exception */ }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }

            return Task.FromResult(returnValue);
        }

        public async Task<WordPressUser> CreateUser(string firstName, 
            string lastName,
            string email,
            string phoneNumber, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = new User();
            user.Email = email;
            user.Name = firstName +" "+ lastName;
            user.NiceName = user.Name.Replace(" ","-").ToLowerInvariant();
            user.UserName = email;
            user.Password = "$P$BOctava";
            user.Url = string.Empty;
            user.UserRegisteredUtc = DateTime.UtcNow;
           
            user.Metadata.Add(new UserMetadata("nickname", email));
            user.Metadata.Add(new UserMetadata("first_name", firstName));
            user.Metadata.Add(new UserMetadata("last_name", lastName));
            user.Metadata.Add(new UserMetadata("description", string.Empty));
            user.Metadata.Add(new UserMetadata("rich_editing", "true"));
            user.Metadata.Add(new UserMetadata("syntax_highlighting", "true"));
            user.Metadata.Add(new UserMetadata("comment_shortcuts", "false"));
            user.Metadata.Add(new UserMetadata("admin_color", "fresh"));
            user.Metadata.Add(new UserMetadata("use_ssl", "0"));
            user.Metadata.Add(new UserMetadata("show_admin_bar_front", "true"));
            user.Metadata.Add(new UserMetadata("locale", string.Empty));
            user.Metadata.Add(new UserMetadata("wp_capabilities", "a:1:{s:10:\"subscriber\";b:1;}"));
            user.Metadata.Add(new UserMetadata("wp_user_level", "0"));
            user.Metadata.Add(new UserMetadata("session_tokens", string.Empty));
            user.Metadata.Add(new UserMetadata("community-events-location", string.Empty));

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

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
