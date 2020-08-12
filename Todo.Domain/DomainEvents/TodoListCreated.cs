using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class TodoListCreated : INotification
    {
        public TodoList List { get; set; }
    }
}
