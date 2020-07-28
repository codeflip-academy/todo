using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.Infrastructure;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationWhenTodoListCompletedStateChangedEventHandler: INotificationHandler<TodoListCompletedStateChanged>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationWhenTodoListCompletedStateChangedEventHandler(IHubContext<NotificationHub> hubContext, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Handle(TodoListCompletedStateChanged notification, CancellationToken cancellationToken)
        {
            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(notification.List, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

           return _hubContext.Clients.Users(contributorExceptYou).SendAsync("ListCompletedStateChanged", notification.List.Id, notification.List.Completed);
        }
    }
}