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

namespace TodoWebAPI.DomainEventHandlers.EditItem
{
    public class SendNotificationWhenItemHadBeenUpdated : INotificationHandler<ItemChanged>
    {
        private readonly ITodoListRepository _todoList;
        private readonly IHubContext<NotificationHub> _context;
        private readonly IAccountRepository _account;

        public SendNotificationWhenItemHadBeenUpdated(ITodoListRepository todoList, IHubContext<NotificationHub> context, IAccountRepository account)
        {
            _todoList = todoList;
            _context = context;
            _account = account;
        }
        public async Task Handle(ItemChanged notification, CancellationToken cancellationToken)
        {
            var list = await _todoList.FindTodoListIdByIdAsync(notification.Item.ListId.GetValueOrDefault());
            var account = await _account.FindAccountByIdAsync(notification.AccountId);

            var contributorsExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, account);

            await _context.Clients.Users(contributorsExceptYou).SendAsync("ItemUpdated", notification.Item);
        }
    }
}
