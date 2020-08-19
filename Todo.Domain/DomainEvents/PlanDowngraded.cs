using System;
using System.Collections.Generic;
using MediatR;

namespace Todo.Domain.DomainEvents
{
    public class PlanDowngraded : INotification
    {
        public Downgrade Downgrade { get; set; }
    }
}