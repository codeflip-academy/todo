using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories
{
    public class MarkItemAsImportantHandler : AsyncRequestHandler<MarkItemAsImportant>
    {
        private readonly ITodoListItemRepository _itemRepository;

        public MarkItemAsImportantHandler(ITodoListItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        protected async override Task Handle(MarkItemAsImportant request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.FindToDoListItemByIdAsync(request.ItemId);

            item.Important = request.Important;

            await _itemRepository.SaveChangesAsync();
        }
    }
}