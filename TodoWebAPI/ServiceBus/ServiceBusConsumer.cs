using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Todo.Infrastructure.Email;

namespace TodoWebAPI.ServiceBus
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly string _serviceBusConnectionString;
        private readonly QueueClient _queueClient;
        private readonly string _queueName;

        public ServiceBusConsumer(IConfiguration configuration, ILoggerFactory loggerFactory, IEmailService emailService)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ServiceBusConsumer>();
            _serviceBusConnectionString = _configuration.GetConnectionString("AzureServiceBus");
            _queueName = "notifications";
            _queueClient = new QueueClient(_serviceBusConnectionString, _queueName);
            _emailService = emailService;
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            _logger.LogTrace("Registering service bus");
            
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 3,
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var email = JsonConvert.DeserializeObject<Email>(Encoding.UTF8.GetString(message.Body));

            await _emailService.SendEmailAsync(email);

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, "Message handler encountered an exception");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogDebug($"- Endpoint: {context.Endpoint}");
            _logger.LogDebug($"- Entity Path: {context.EntityPath}");
            _logger.LogDebug($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}