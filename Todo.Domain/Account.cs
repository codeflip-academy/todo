using System;
using System.Collections.Generic;
using System.Text;
using Todo.Core;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class Account : Entity
    {
        public Account()
        {

        }
        public Account(Guid accountId, string email)
        {
            Id = accountId;
            Email = email;

            DomainEvents.Add(new AccountCreated() { AccountId = Id, PlanId = PlanTiers.Free });
        }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public string Email { get; set; }
        public string CustomerId { get; set; }
        public string PaymentMethodId { get; set; }
        public string SubscriptionId { get; private set; }
        public string PaymentMethodDeletedPlan { get; set; }
        public bool HasSubscription() => !string.IsNullOrEmpty(SubscriptionId);
        public bool HasPaymentMethod() => !string.IsNullOrEmpty(PaymentMethodId);
        public bool HasPaymentMethodDeletedPlan() => !string.IsNullOrEmpty(PaymentMethodDeletedPlan);
        public void UpdateSubscriptionId(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
        public void RemoveSubscriptionId()
        {
            SubscriptionId = null;
        }

        public void RemovePaymentMethodDeletedPlan()
        {
            PaymentMethodDeletedPlan = null;
        }

        public void RemovePaymentMethodId()
        {
            PaymentMethodId = null;
        }
    }
}
