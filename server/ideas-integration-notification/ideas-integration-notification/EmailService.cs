using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Notification
{
    internal class EmailService : IEmailService
    {

        public EmailService(INodeServices nodeServices,
            string smtpHost,
            string fromAddress,
            string fromDisplayName)
        {
            _nodeServices = nodeServices;
            _smtpHost = smtpHost;
            _fromAddress = fromAddress;
            _fromDisplayName = fromDisplayName;
        }

        private readonly INodeServices _nodeServices;
        private readonly string _smtpHost;
        private readonly string _fromAddress;
        private readonly string _fromDisplayName;

        protected virtual async Task<string> Render(string template, string ideaDataSerialized)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;
            var templateFormatted = template.Replace("<<", "{{").Replace(">>", "}}");

            return await _nodeServices.InvokeAsync<string>("handlebarsRenderer.js", templateFormatted, ideaDataSerialized);
        }

        public async Task SendEmailAsync(dynamic mergeTemplate, IDictionary<string, object> ideaData)
        {
            try
            {
                var data = mergeTemplate["data"];
                if (data != null)
                {
                    string ideaDataSerialized = JsonConvert.SerializeObject(ideaData);
                    string to = await Render((string)data["to"], ideaDataSerialized);
                    string cc = await Render((string)data["cc"], ideaDataSerialized);
                    string bcc = await Render((string)data["bcc"], ideaDataSerialized);
                    string subject = await Render((string)data["subject"], ideaDataSerialized);
                    string body = await Render((string)data["body"], ideaDataSerialized);

                    var smtpClient = new SmtpClient();
                    smtpClient.Host = _smtpHost;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(_fromAddress, _fromDisplayName);
                    if (!string.IsNullOrWhiteSpace(to))
                    mailMessage.To.Add(to);
                    if (!string.IsNullOrWhiteSpace(cc))
                        mailMessage.CC.Add(cc);
                    if (!string.IsNullOrWhiteSpace(bcc))
                        mailMessage.Bcc.Add(bcc);
                    mailMessage.Body = body;
                    mailMessage.Subject = subject;
                    mailMessage.IsBodyHtml = true;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception err)
            {
                Trace.TraceError($"Error calling renderer: { err.Message }");
                throw;
            }
        }
    }
}
