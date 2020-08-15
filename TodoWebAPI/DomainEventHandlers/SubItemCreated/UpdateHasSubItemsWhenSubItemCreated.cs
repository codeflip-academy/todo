using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace TodoWebAPI.DomainEventHandlers
{
    public class UpdateHasSubItemsStateWhenSubItemCreated : INotificationHandler<SubItemCreated>
    {
        private readonly ITodoListItemRepository _todoListItemRepository;
        private readonly ISubItemRepository _subItemRepository;

        public UpdateHasSubItemsStateWhenSubItemCreated(ITodoListItemRepository todoListItemRepository, ISubItemRepository subItemRepository)
        {
            _todoListItemRepository = todoListItemRepository;
            _subItemRepository = subItemRepository;
        }
        public async Task Handle(SubItemCreated notification, CancellationToken cancellationToken)
        {
            var itemId = notification.SubItem.ListItemId.GetValueOrDefault();
            var item = await _todoListItemRepository.FindToDoListItemByIdAsync(itemId);

            item.SetHasSubItems(true);

            await _todoListItemRepository.SaveChangesAsync();
        }
    }
}