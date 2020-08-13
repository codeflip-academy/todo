using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core
{
    public abstract class Entity
    {
        public List<INotification> DomainEvents { get; } = new List<INotification>();

        public void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }

    }
}
