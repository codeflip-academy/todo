using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class ListNameUpdated : INotification
    {
        public TodoList List { get; set; }
        public Guid AccountId {get; set;}
    }
}
