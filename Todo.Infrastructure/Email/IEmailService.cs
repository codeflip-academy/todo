using System;
using System.Threading.Tasks;
using TodoWebAPI;

namespace Todo.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage email);
    }
}