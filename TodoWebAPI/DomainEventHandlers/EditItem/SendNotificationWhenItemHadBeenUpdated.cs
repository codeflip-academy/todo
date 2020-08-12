using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using TodoWebAPI.SignalR;
using Todo.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace TodoWebAPI.DomainEventHandlers.EditItem
{
    public class SendNotificationWhenItemHadBeenUpdated : INotificationHandler<ItemChanged>
    {
        private readonly ITodoListRepository _todoList;
        private readonly IHubContext<NotificationHub> _context;
        private readonly IAccountRepository _account;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationWhenItemHadBeenUpdated(ITodoListRepository todoList, IHubContext<NotificationHub> context, IAccountRepository account, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _todoList = todoList;
            _context = context;
            _account = account;
        }
        public async Task Handle(ItemChanged notification, CancellationToken cancellationToken)
        {
            var list = await _todoList.FindTodoListIdByIdAsync(notification.Item.ListId.GetValueOrDefault());

            var usersExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _context.Clients.Users(usersExceptYou).SendAsync("ItemUpdated", notification.Item);
        }
    }
}
