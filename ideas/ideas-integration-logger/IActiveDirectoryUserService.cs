using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace CoE.Ideas.Integration.Logger
{
    public interface IActiveDirectoryUserService
    {
        UserPrincipal GetADUser(string email);
    }
}
