using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Email;
using TodoWebAPI.ServiceBus;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendEmailWhenListIsCompletedDomainEventHandler : INotificationHandler<TodoListCompletedStateChanged>
    {
        private readonly IEmailQueue _emailQueue;
        private readonly IConfiguration _config;
        private readonly DapperQuery _dapper;
        private readonly ILogger _logger;
        private readonly IAccountRepository _account;
        private readonly IAccountPlanRepository _accountPlan;

        public SendEmailWhenListIsCompletedDomainEventHandler(IConfiguration config,
         DapperQuery dapper,
          IEmailQueue emailQueue,
           ILogger<SendEmailWhenListIsCompletedDomainEventHandler> logger,
           IAccountRepository account,
           IAccountPlanRepository accountPlan)
        {
            _config = config;
            _dapper = dapper;
            _emailQueue = emailQueue;
            _logger = logger;
            _account = account;
            _accountPlan = accountPlan;
        }
        public async Task Handle(TodoListCompletedStateChanged notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {notification.List.Id} completed state changed. Completed = '{notification.List.Completed}'.");

            var list = notification.List;
            var emails = await _dapper.GetEmailsFromAccountsByListIdAsync(list.Id);

            var messages = new List<EmailMessage>();

            if (list.Completed)
            {
                foreach (var userEmail in emails)
                {
                    var account = await _account.FindAccountByEmailAsync(userEmail);
                    var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(account.Id);

                    if (accountPlan.PlanId == PlanTiers.Basic || accountPlan.PlanId == PlanTiers.Premium && account.EmailCompleted == true)
                    {
                        messages.Add(
                            new EmailMessage()
                            {
                                To = userEmail,
                                From = _config.GetSection("Emails")["Notifications"],
                                Subject = $"You completed a list!",
                                Body = $"{list.ListTitle} is completed! Nice work!"
                            });
                    }
                }

                _logger.LogInformation($"Sending email notification for completed list {list.Id}.");

                await _emailQueue.QueueEmailAsync(messages);
            }
        }
    }
}