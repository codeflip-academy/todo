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

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationToClientWheneverSubItemIsUpdatedHandler : INotificationHandler<EditSubItem>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IAccountRepository _account;
        private readonly ITodoListItemRepository _todoListItem;
        private readonly ITodoListRepository _todoList;

        public SendNotificationToClientWheneverSubItemIsUpdatedHandler(IHttpContextAccessor httpContextAccessor, IHubContext<NotificationHub> hubContext,IAccountRepository account, ITodoListItemRepository todoListItem, ITodoListRepository todoList)
        {
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _account = account;
            _todoListItem = todoListItem;
            _todoList = todoList;
        }
        public async Task Handle(EditSubItem notification, CancellationToken cancellationToken)
        {
            var item = await _todoListItem.FindToDoListItemByIdAsync(notification.SubItem.ListItemId.GetValueOrDefault());
            var list = await _todoList.FindTodoListIdByIdAsync(item.ListId.GetValueOrDefault());

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hubContext.Clients.Users(contributorExceptYou).SendAsync("SubItemUpdated", notification.SubItem);
        }
    }
}
