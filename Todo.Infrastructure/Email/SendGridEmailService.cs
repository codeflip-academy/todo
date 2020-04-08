using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {
        public async Task SendEmailAsync(EmailModel email)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(email.From);
            var subject = email.Subject;
            var to = new EmailAddress(email.To);
            var plainTextContent = email.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, null);
            var response = await client.SendEmailAsync(msg);
        }
    }
}