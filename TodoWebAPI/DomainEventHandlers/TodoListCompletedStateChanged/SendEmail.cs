using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Email;
using Todo.Infrastructure.ServiceBus;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendEmail : INotificationHandler<TodoListCompletedStateChanged>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IServiceBusEmail _serviceBusEmail;

        public SendEmail(IAccountRepository accountRepository, 
            ITodoListRepository todoListRepository,
            IEmailService emailService,
            IConfiguration config, IServiceBusEmail serviceBusEmail)
        {
            _accountRepository = accountRepository;
            _todoListRepository = todoListRepository;
            _emailService = emailService;
            _config = config;
            _serviceBusEmail = serviceBusEmail;
        }
        public async Task Handle(TodoListCompletedStateChanged notification, CancellationToken cancellationToken)
        {
            var list = notification.List;

            if (list.Completed)
            {
                var account = await _accountRepository.FindAccountByIdAsync(list.AccountId);

                var email = new EmailModel()
                {
                    To = account.Email,
                    From = _config.GetSection("Emails")["Notifications"],
                    Subject = $"You finished a list!",
                    Body = $"List {list.ListTitle} is finished! Nice work!"
                };

                _serviceBusEmail.SendServiceBusEmail(email);
            }
        }
    }
}
