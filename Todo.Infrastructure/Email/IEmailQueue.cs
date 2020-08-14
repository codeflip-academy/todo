using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Infrastructure.Email;

namespace TodoWebAPI.ServiceBus
{
    public interface IEmailQueue
    {
        Task QueueEmailAsync(List<EmailMessage> emails);
    }
}