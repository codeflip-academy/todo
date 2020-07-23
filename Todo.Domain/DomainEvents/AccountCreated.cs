using System;
using MediatR;

namespace Todo.Domain.DomainEvents
{
    public class AccountCreated : INotification
    {
        public Guid AccountId { get; set; }
        public int PlanId { get; set; }
    }
}
