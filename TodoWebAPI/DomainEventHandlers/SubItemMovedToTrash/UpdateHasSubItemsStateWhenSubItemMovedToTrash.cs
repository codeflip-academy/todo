using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace TodoWebAPI.DomainEventHandlers
{
    public class UpdateHasSubItemsStateWhenSubItemMovedToTrash : INotificationHandler<SubItemMovedToTrash>
    {
        private readonly ITodoListItemRepository _todoListItemRepository;
        private readonly ISubItemRepository _subItemRepository;

        public UpdateHasSubItemsStateWhenSubItemMovedToTrash(ITodoListItemRepository todoListItemRepository, ISubItemRepository subItemRepository)
        {
            _todoListItemRepository = todoListItemRepository;
            _subItemRepository = subItemRepository;
        }
        public async Task Handle(SubItemMovedToTrash notification, CancellationToken cancellationToken)
        {
            var item = await _todoListItemRepository.FindToDoListItemByIdAsync(notification.ItemId.GetValueOrDefault());

            var subItems = await _subItemRepository.FindAllSubItemsByListItemIdAsync(item.Id);

            item.SetHasSubItems(subItems.Count > 0);

            await _todoListItemRepository.SaveChangesAsync();
        }
    }
}