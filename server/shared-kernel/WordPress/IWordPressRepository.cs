using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.WordPress
{
    public interface IWordPressRepository
    {
        WordPressUser GetUserAsync(int id);
    }
}
