using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Todo.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Todo.Domain;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TodoWebAPI.ServiceBus
{
    public class ServiceBusEmail : IServiceBusEmail
    {
        const string QueueName = "notifications";
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private QueueClient _queueClient;

        public ServiceBusEmail(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ServiceBusEmail>();
        }

        public async void SendServiceBusEmail(List<Email> emails)
        {
            var connectionString = _configuration.GetSection("ConnectionStrings")["AzureServiceBus"];
            _queueClient = new QueueClient(connectionString, QueueName);

            try
            {
                foreach (var email in emails)
                {
                    var messageBody = JsonConvert.SerializeObject(email);
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    await _queueClient.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await _queueClient.CloseAsync();
            }
        }
    }
}