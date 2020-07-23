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

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationWhenSubItemMovedToTrashEventHandler : INotificationHandler<SubItemMovedToTrash>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListItemRepository _todoListItemRepository;
        private readonly ITodoListRepository _todoListRepository;

        public SendNotificationWhenSubItemMovedToTrashEventHandler(IHubContext<NotificationHub> hubContext, ITodoListItemRepository todoListItem, ITodoListRepository todoList)
        {
            _hubContext = hubContext;
            _todoListItemRepository = todoListItem;
            _todoListRepository = todoList;
        }
        public async Task Handle(SubItemMovedToTrash notification, CancellationToken cancellationToken)
        {
            var item = await _todoListItemRepository.FindToDoListItemByIdAsync(notification.ItemId.GetValueOrDefault());
            var list = await _todoListRepository.FindTodoListIdByIdAsync(item.ListId.GetValueOrDefault());

            await _hubContext.Clients.Users(list.Contributors).SendAsync("SubItemTrashed", notification.ItemId, notification.SubItem);
        }
    }
}
