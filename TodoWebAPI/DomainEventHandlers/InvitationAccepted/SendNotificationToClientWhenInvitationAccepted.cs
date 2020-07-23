using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using TodoWebAPI.SignalR;
using Todo.Infrastructure;

namespace TodoWebAPI.DomainEventHandlers.Invitation
{
    public class SendNotificationToClientWhenInvitationAccepted : INotificationHandler<InvitationAccepted>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IAccountRepository _account;

        public SendNotificationToClientWhenInvitationAccepted(IHubContext<NotificationHub> hubContext, IAccountRepository account)
        {
            _hubContext = hubContext;
            _account = account;
        }
        public async Task Handle(InvitationAccepted notification, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(notification.AccountId);

            var contribuutorsExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(notification.List, account);

            await _hubContext.Clients.Users(contribuutorsExceptYou).SendAsync("InvitationAccepted", notification.List);
        }
    }
}
