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

namespace TodoWebAPI.DomainEventHandlers.Layouts
{
    public class SendNotificationToClientWhenLayoutIsChanged : INotificationHandler<ListLayoutUpdated>
    {
        private readonly IHubContext<NotificationHub> _context;
        private readonly ITodoListRepository _todoList;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationToClientWhenLayoutIsChanged(IHubContext<NotificationHub> context, ITodoListRepository todoList, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _todoList = todoList;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(ListLayoutUpdated notification, CancellationToken cancellationToken)
        {
            var list = await _todoList.FindTodoListIdByIdAsync(notification.ListId);

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _context.Clients.Users(contributorExceptYou).SendAsync("ListLayoutChanged", notification.ListId);
        }
    }
}
