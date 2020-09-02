using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Todo.Domain;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Email;
using TodoWebAPI.ServiceBus;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendEmailWhenItemIsCompleted : INotificationHandler<TodoListItemCompletedStateChanged>
    {
        private readonly DapperQuery _dapper;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IEmailQueue _emailQueue;
        private readonly IAccountRepository _account;

        public SendEmailWhenItemIsCompleted(IAccountRepository account, DapperQuery dapper, IAccountPlanRepository accountPlan, IConfiguration config, ILogger<SendEmailWhenItemIsCompleted> logger, IEmailQueue emailQueue)
        {
            _dapper = dapper;
            _accountPlan = accountPlan;
            _config = config;
            _logger = logger;
            _emailQueue = emailQueue;
            _account = account;
        }
        public async Task Handle(TodoListItemCompletedStateChanged notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {notification.Item.Id} completed state changed. Completed = '{notification.Item.Completed}'.");

            var item = notification.Item;
            var emails = await _dapper.GetEmailsFromAccountsByListIdAsync(item.ListId.GetValueOrDefault());

            var messages = new List<EmailMessage>();

            if (item.Completed)
            {
                foreach (var userEmail in emails)
                {
                    var account = await _account.FindAccountByEmailAsync(userEmail);
                    var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(account.Id);

                    if (accountPlan.PlanId == PlanTiers.Basic || accountPlan.PlanId == PlanTiers.Premium && account.EmailItemCompleted == true)
                    {
                        messages.Add(
                            new EmailMessage()
                            {
                                To = userEmail,
                                From = _config.GetSection("Emails")["Notifications"],
                                Subject = $"You completed a item!",
                                Body = $"{item.Name} is completed! Nice work!"
                            });
                    }
                }

                _logger.LogInformation($"Sending email notification for completed item {item.Id}.");

                await _emailQueue.QueueEmailAsync(messages);
            }
        }
    }
}