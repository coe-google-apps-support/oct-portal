using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    internal class EmailService : IEmailService
    {

        public Task SendEmailAsync(dynamic mergeTemplate, IDictionary<string, object> ideaData)
        {
            throw new NotImplementedException();
        }
    }
}
