using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class ItemLayoutUpdated : INotification
    {
        public int Position { get; set; }
        public Guid SubItemId { get; set; }
        public Guid ItemId { get; set; }
        public Guid AccountId {get; set;}
    }
}
