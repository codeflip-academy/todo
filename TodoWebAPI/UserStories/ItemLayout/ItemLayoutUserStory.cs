using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories.ItemLayout
{
    public class ItemLayoutUserStory : AsyncRequestHandler<ItemLayout>
    {
        private readonly ISubItemLayoutRepository _subItemLayout;

        public ItemLayoutUserStory(ISubItemLayoutRepository subItemLayout)
        {
            _subItemLayout = subItemLayout;
        }
        protected override async Task Handle(ItemLayout request, CancellationToken cancellationToken)
        {
            var layout = await _subItemLayout.FindLayoutByListItemIdAsync(request.ItemId);

            layout.AccountId = request.AccountId;

            layout.UpdateLayout(request.SubItemId, request.Position, request.ItemId);

            _subItemLayout.Update(layout);
            await _subItemLayout.SaveChangesAsync();
        }
    }
}
