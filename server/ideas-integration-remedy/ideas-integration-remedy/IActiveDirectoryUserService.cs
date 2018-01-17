using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace CoE.Ideas.Remedy
{
    public interface IActiveDirectoryUserService
    {
        UserPrincipal GetADUser(string email);
    }
}
