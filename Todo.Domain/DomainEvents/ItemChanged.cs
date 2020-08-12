using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class ItemChanged : INotification
    {
        public TodoListItem Item { get; set; }
    }
}
