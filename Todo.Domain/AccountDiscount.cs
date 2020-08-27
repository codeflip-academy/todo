using System;
using Todo.Core;

namespace Todo.Domain
{
    public class AccountDiscount : Entity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public int DiscountId { get; set; }
        public bool AppliedToSubscription { get; set; }
        public void ApplyDiscountToSubscription() => AppliedToSubscription = true;
    }
}