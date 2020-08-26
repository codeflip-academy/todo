using System;
using MediatR;

namespace Todo.Domain.DomainEvents
{
    public class TrashedList : INotification
    {
        public Guid ListId { get; set; }
    }
}