using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationWhenContributorLeaves : INotificationHandler<ContributorLeft>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IAccountRepository _accounts;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationWhenContributorLeaves(
            IHubContext<NotificationHub> hubContext,
            ITodoListRepository todoListRepository,
            IAccountRepository accountsLists,
            IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
            _accounts = accountsLists;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(ContributorLeft notification, CancellationToken cancellationToken)
        {
            var list = await _todoListRepository.FindTodoListIdByIdAsync(notification.ListId);

            var usersExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hubContext.Clients.Users(usersExceptYou).SendAsync("ContributorLeft", list);
        }

        
    }
}
