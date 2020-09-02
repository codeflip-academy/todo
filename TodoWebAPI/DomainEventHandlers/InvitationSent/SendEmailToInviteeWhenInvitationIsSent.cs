using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Email;
using TodoWebAPI.ServiceBus;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendEmailToInviteeWhenInvitationIsSent : INotificationHandler<InvitationSent>
    {
        private readonly IAccountRepository _account;
        private readonly ILogger<InvitationSent> _logger;
        private readonly IConfiguration _config;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IEmailQueue _emailQueue;

        public SendEmailToInviteeWhenInvitationIsSent(IAccountRepository account, ILogger<InvitationSent> logger, IConfiguration config, IAccountPlanRepository accountPlan, IEmailQueue emailQueue)
        {
            _account = account;
            _logger = logger;
            _config = config;
            _accountPlan = accountPlan;
            _emailQueue = emailQueue;
        }
        public async Task Handle(InvitationSent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {notification.InviteeAccountId} Invitation sent '{notification.InviteeAccountId}'.");

            var messages = new List<EmailMessage>();

            var inviteeAccount = await _account.FindAccountByIdAsync(notification.InviteeAccountId);
            var inviteeAccountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(inviteeAccount.Id);

            if (inviteeAccountPlan.PlanId == PlanTiers.Basic || inviteeAccountPlan.PlanId == PlanTiers.Premium && inviteeAccount.EmailInvitation == true)
            {
                messages.Add(
                    new EmailMessage()
                    {
                        To = inviteeAccount.Email,
                        From = _config.GetSection("Emails")["Notifications"],
                        Subject = $"You're invited to a list.",
                        Body = $"You've been sent an invitation to a list! Go check it out!"
                    });
            }

            _logger.LogInformation($"Sending email notification for Invitation sent.");

            await _emailQueue.QueueEmailAsync(messages);
        }
    }
}