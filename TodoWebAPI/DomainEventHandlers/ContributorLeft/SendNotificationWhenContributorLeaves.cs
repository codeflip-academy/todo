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

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationWhenContributorLeaves : INotificationHandler<ContributorLeft>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListRepository _todoListRepository;

        public SendNotificationWhenContributorLeaves(
            IHubContext<NotificationHub> hubContext,
            ITodoListRepository todoListRepository)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
        }
        public async Task Handle(ContributorLeft notification, CancellationToken cancellationToken)
        {
            var list = await _todoListRepository.FindTodoListIdByIdAsync(notification.ListId);

            await _hubContext.Clients.Users(list.Contributors).SendAsync("ContributorLeft", list);
        }
    }
}
