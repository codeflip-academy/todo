using System;
using System.Collections.Generic;
using Todo.Domain;
using Todo.Infrastructure.Email;

namespace TodoWebAPI.ServiceBus
{
    public interface IServiceBusEmail
    {
        void SendServiceBusEmail(List<Email> emails);
    }
}