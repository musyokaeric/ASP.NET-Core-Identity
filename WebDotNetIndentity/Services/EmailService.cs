using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using WebDotNetIndentity.Settings;

namespace WebDotNetIndentity.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSettings> smtpSetting;

        public EmailService(IOptions<SmtpSettings> smtpSetting)
        {
            this.smtpSetting = smtpSetting;
        }

        public async Task SendAsync(string from, string to, string subject, string body)
        {
            // Send Email
            var message = new MailMessage(from, to, subject, body);

            using (var emailClient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(smtpSetting.Value.UserName, smtpSetting.Value.Password);
                await emailClient.SendMailAsync(message);
            }
        }
    }
}
