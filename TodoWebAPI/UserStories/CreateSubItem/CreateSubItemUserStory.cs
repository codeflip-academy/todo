using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories.CreateSubItem
{
    public class CreateSubItemUserStory : IRequestHandler<CreateSubItem, SubItem>
    {
        private readonly ITodoListItemRepository _listItems;
        private readonly ISubItemRepository _subItems;

        public CreateSubItemUserStory(ITodoListItemRepository listItems, ISubItemRepository subItems)
        {
            _listItems = listItems;
            _subItems = subItems;
        }

        public async Task<SubItem> Handle(CreateSubItem request, CancellationToken cancellationToken)
        {
            var item = await _listItems.FindToDoListItemByIdAsync(request.ListItemId);
            var subItem = item.CreateSubItem(request.Name);

            subItem.AccountId = request.AccountId;

            subItem.Id = _subItems.NextId();

            _subItems.Add(subItem);

            await _subItems.SaveChangesAsync();

            return subItem;
        }
    }
}
