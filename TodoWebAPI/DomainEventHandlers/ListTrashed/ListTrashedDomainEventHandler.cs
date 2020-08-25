using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace TodoWebAPI.DomainEventHandlers.ListTrashed
{
    public class ListTrashedDomainEventHandler : INotificationHandler<TrashedList>
    {
        private readonly ITodoListItemRepository _itemRepository;
        private readonly ISubItemRepository _subItem;

        public ListTrashedDomainEventHandler(ITodoListItemRepository itemRepository, ISubItemRepository subItem)
        {
            _itemRepository = itemRepository;
            _subItem = subItem;
        }
        public async Task Handle(TrashedList notification, CancellationToken cancellationToken)
        {
            var itemsInList = await _itemRepository.FindAllTodoListItemsByListIdAsync(notification.ListId);

            foreach (var item in itemsInList)
            {
                var subitems = await _subItem.FindAllSubItemsByListItemIdAsync(item.Id);

                foreach (var subItem in subitems)
                {
                    subItem.MoveToTrash();
                }

                item.MoveToTrash();
            }

            await _itemRepository.SaveChangesAsync();
        }
    }
}