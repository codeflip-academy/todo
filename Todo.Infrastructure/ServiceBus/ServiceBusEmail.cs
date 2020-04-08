using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Infrastructure.Email;

namespace Todo.Infrastructure.ServiceBus
{
    public class ServiceBusEmail : IServiceBusEmail
    {
        public void SendServiceBusEmail(EmailModel email)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                byte[] messagebuffer = Encoding.Default.GetBytes(JsonConvert.SerializeObject(email));

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: messagebuffer);
            }
        }
    }
}
