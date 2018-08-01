using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    public interface IEmailService
    {
        Task SendEmailAsync(dynamic mergeTemplate, IDictionary<string, object> ideaData);
    }
}
