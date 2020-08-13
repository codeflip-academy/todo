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

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendInvitationToClientWhenSubItemIsCreated : INotificationHandler<SubItemCreated>
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ITodoListItemRepository _todoListItem;
        private readonly ITodoListRepository _todoList;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendInvitationToClientWhenSubItemIsCreated(IHubContext<NotificationHub> hub, ITodoListItemRepository todoListItem, ITodoListRepository todoList, IHttpContextAccessor httpContextAccessor)
        {
            _hub = hub;
            _todoListItem = todoListItem;
            _todoList = todoList;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(SubItemCreated notification, CancellationToken cancellationToken)
        {
            var item = await _todoListItem.FindToDoListItemByIdAsync(notification.SubItem.ListItemId.GetValueOrDefault());
            var list = await _todoList.FindTodoListIdByIdAsync(item.ListId.GetValueOrDefault());

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hub.Clients.Users(contributorExceptYou).SendAsync("SubItemCreated", notification.SubItem);
        }
    }
}
