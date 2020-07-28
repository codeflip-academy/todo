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
using Microsoft.AspNetCore.Http;

namespace TodoWebAPI.DomainEventHandlers.Invitation
{
    public class SendNotificationToClientWhenInvitationAccepted : INotificationHandler<InvitationAccepted>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IAccountRepository _account;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationToClientWhenInvitationAccepted(IHubContext<NotificationHub> hubContext, IAccountRepository account, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _account = account;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(InvitationAccepted notification, CancellationToken cancellationToken)
        {
            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(notification.List, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hubContext.Clients.Users(contributorExceptYou).SendAsync("InvitationAccepted", notification.List);
        }
    }
}
