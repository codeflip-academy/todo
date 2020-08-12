using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class TodoListItemCreated : INotification
    {
        public TodoListItem Item { get; set; }
        public TodoList List { get; set; }
    }
}
