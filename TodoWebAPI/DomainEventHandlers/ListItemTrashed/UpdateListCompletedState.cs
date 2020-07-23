using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace TodoWebAPI.DomainEventHandlers.ListItemTrashed
{
    public class UpdateListCompletedState : INotificationHandler<ItemMovedToTrash>
    {
        private readonly ITodoListItemRepository _itemRepository;
        private readonly ITodoListRepository _listRepository;

        public UpdateListCompletedState(ITodoListItemRepository itemRepository, ITodoListRepository listRepository)
        {
            _itemRepository = itemRepository;
            _listRepository = listRepository;
        }
        public async Task Handle(ItemMovedToTrash notification, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.FindAllTodoListItemsByListIdAsync(notification.ListId.GetValueOrDefault());
            var list = await _listRepository.FindTodoListIdByIdAsync(notification.ListId.GetValueOrDefault());

            list.SetCompleted(items);

            await _listRepository.SaveChangesAsync();

        }
    }
}
