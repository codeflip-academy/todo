using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Todo.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(ILogger<SendGridEmailService> logger)
        {
            _logger = logger;
        }
        public async Task SendEmailAsync(EmailMessage email)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(email.From);
            var subject = email.Subject;
            var to = new EmailAddress(email.To);
            var plainTextContent = email.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, null);
            try
            {
                _logger.LogInformation("sending email to send grid.");
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var content = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Error when calling sendgrid api: " + content);
                }

                _logger.LogInformation("email sent to sendgrid.");
            }

            catch(Exception ex)
            {
                _logger.LogError(ex, "Error when calling sendgrid api.");
            }
            
        }
    }
}