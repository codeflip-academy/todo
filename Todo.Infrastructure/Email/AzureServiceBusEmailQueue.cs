using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Todo.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Todo.Domain;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TodoWebAPI.ServiceBus
{
    public class AzureServiceBusEmailQueue : IEmailQueue
    {
        const string QueueName = "notifications";
        private readonly ILogger _logger;
        private readonly QueueClient _queueClient;

        public AzureServiceBusEmailQueue(IConfiguration configuration, ILogger<AzureServiceBusEmailQueue> logger)
        {
            var connectionString = configuration.GetSection("ConnectionStrings")["AzureServiceBus"];
            _queueClient = new QueueClient(connectionString, QueueName);

            _logger = logger;
        }

        public async Task QueueEmailAsync(List<EmailMessage> emails)
        {
            try
            {
                var messages = new List<Message>();

                foreach (var email in emails)
                {
                    var messageBody = JsonConvert.SerializeObject(email);
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    messages.Add(message);
                }

                await _queueClient.SendAsync(messages);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}