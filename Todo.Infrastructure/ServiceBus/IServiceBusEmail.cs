using System;
using System.Collections.Generic;
using System.Text;
using Todo.Infrastructure.Email;

namespace Todo.Infrastructure.ServiceBus
{
    public interface IServiceBusEmail
    {
        void SendServiceBusEmail(EmailModel email);
    }
}
