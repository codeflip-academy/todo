using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class ListLayoutUpdated : INotification
    {
        public Guid ListId { get; set; }
        public Guid ItemId { get; set; }
        public int Position { get; set; }
    }
}
