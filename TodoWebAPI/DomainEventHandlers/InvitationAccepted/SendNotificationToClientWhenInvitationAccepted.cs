using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers.Invitation
{
    public class SendNotificationToClientWhenInvitationAccepted : INotificationHandler<InvitationAccepted>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SendNotificationToClientWhenInvitationAccepted(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public Task Handle(InvitationAccepted notification, CancellationToken cancellationToken)
        {
            return _hubContext.Clients.Users(notification.List.Contributors).SendAsync("InvitationAccepted", notification.List);
        }
    }
}
