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
        public bool EmailDueDate { get; set; }
        public bool EmailListCompleted { get; set; }
        public bool EmailItemCompleted { get; set; }
        public bool EmailInvitation { get; set; }

        public void FilterEmails(bool emailDueDate, bool emailCompleted, bool emailItemCompleted, bool emailInvitation)
        {
            this.EmailDueDate = emailDueDate;
            this.EmailListCompleted = emailCompleted;
            this.EmailItemCompleted = emailItemCompleted;
            this.EmailInvitation = emailInvitation;
        }
        public string SubscriptionId { get; private set; }
        public bool HasSubscription() => !string.IsNullOrEmpty(SubscriptionId);
        public bool HasPaymentMethod() => !string.IsNullOrEmpty(PaymentMethodId);
        public void UpdateSubscriptionId(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
