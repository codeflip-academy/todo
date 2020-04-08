using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.WebAPI.ApplicationServices;

namespace TodoWebAPI.DomainEventHandlers
{
    public class MarkListAsCompleted : INotificationHandler<TodoListItemCompleted>
    {
        private readonly TodoListApplicationService _todoListApplicationService;

        public MarkListAsCompleted(TodoListApplicationService todoListService)
        {
            _todoListApplicationService = todoListService;
        }
        public Task Handle(TodoListItemCompleted notification, CancellationToken cancellationToken)
        {
            return _todoListApplicationService.MarkTodoListAsCompletedAsync(notification.ListItem.ListId);
        }
    }
}
