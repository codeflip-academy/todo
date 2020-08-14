using System;
using System.Threading.Tasks;

namespace TodoWebAPI.ServiceBus
{
    public interface IServiceBusConsumer
    {
        void RegisterOnMessageHandlerAndReceiveMessages();
    }
}