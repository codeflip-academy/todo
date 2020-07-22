using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class ItemMovedToTrash : INotification
    {
        public TodoListItem Item { get; set; }
        public Guid? ListId { get; set; }
        public Guid AccountId {get; set;}
    }
}
