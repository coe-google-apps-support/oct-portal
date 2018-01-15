using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Logger
{
    public class ActiveDirectoryUserService : IActiveDirectoryUserService, IDisposable
    {
        public ActiveDirectoryUserService(string domain, string serviceUserName, string servicePassword)
        {
            principalContext = new PrincipalContext(
                 ContextType.Domain, domain, serviceUserName, servicePassword);
        }

        private PrincipalContext principalContext;

        public UserPrincipal GetADUser(string email)
        {
            return UserPrincipal.FindByIdentity(principalContext, 
                IdentityType.UserPrincipalName, email);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (principalContext != null)
                    {
                        try { principalContext.Dispose(); }
                        catch (Exception) { }
                        finally { principalContext = null; }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ActiveDirectoryUserService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
