using System;
using System.Collections.Generic;
using Todo.Core;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class Downgrade : Entity
    {
        public Guid AccountId { get; set; }
        public DateTime BillingCycleEnd { get; set; }
        public int PlanId { get; set; }

        public void Downgraded()
        {
            DomainEvents.Add(new PlanDowngraded { Downgrade = this });
        }
    }
}