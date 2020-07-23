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


namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationToClientWheneverSubItemIsUpdatedHandler : INotificationHandler<EditSubItem>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IAccountRepository _account;
        private readonly ITodoListItemRepository _todoListItem;
        private readonly ITodoListRepository _todoList;

        public SendNotificationToClientWheneverSubItemIsUpdatedHandler(IHubContext<NotificationHub> hubContext,IAccountRepository account, ITodoListItemRepository todoListItem, ITodoListRepository todoList)
        {
            _hubContext = hubContext;
            _account = account;
            _todoListItem = todoListItem;
            _todoList = todoList;
        }
        public async Task Handle(EditSubItem notification, CancellationToken cancellationToken)
        {
            var item = await _todoListItem.FindToDoListItemByIdAsync(notification.SubItem.ListItemId.GetValueOrDefault());
            var list = await _todoList.FindTodoListIdByIdAsync(item.ListId.GetValueOrDefault());
            var account = await _account.FindAccountByIdAsync(notification.AccountId);

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, account);

            await _hubContext.Clients.Users(contributorExceptYou).SendAsync("SubItemUpdated", notification.SubItem);
        }
    }
}
