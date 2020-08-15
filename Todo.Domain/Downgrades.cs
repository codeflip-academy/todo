using System;
using Todo.Core;

namespace Todo.Domain
{
    public class Downgrade : Entity
    {
        public Guid AccountId { get; set; }
        public DateTime? BillingCycleEnd { get; set; }
        public int PlanId { get; set; }
    }
}