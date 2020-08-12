using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class SubItemCreated : INotification
    {
        public SubItem SubItem { get; set; }
    }
}
