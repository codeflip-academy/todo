using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers.ListItemCompleted
{
    public class SendNotificationToClientWhenItemCompletedHandler : INotificationHandler<TodoListItemCompletedStateChanged>
    {
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationToClientWhenItemCompletedHandler(IHubContext<NotificationHub> hubContext, ITodoListRepository todoListRepository, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(TodoListItemCompletedStateChanged notification, CancellationToken cancellationToken)
        {
            var todoList = await _todoListRepository.FindTodoListIdByIdAsync(notification.Item.ListId.GetValueOrDefault());

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(todoList, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hubContext.Clients.Users(contributorExceptYou).SendAsync("ItemCompleted", notification.Item);
        }
    }
}
