using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Repositories;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories.ListLayout
{
    public class ListLayoutUserStory : AsyncRequestHandler<ListLayout>
    {
        private readonly ITodoListLayoutRepository _todoListLayout;
        public ListLayoutUserStory(ITodoListLayoutRepository todoListLayout)
        {
            _todoListLayout = todoListLayout;
        }
        protected override async Task Handle(ListLayout request, CancellationToken cancellationToken)
        {
            var todoListLayout = await _todoListLayout.FindLayoutByListIdAsync(request.ListId);

            todoListLayout.UpdateLayout(request.ItemId, request.Position, request.ListId);

            _todoListLayout.Update(todoListLayout);
            await _todoListLayout.SaveChangesAsync();
        }
    }
}
