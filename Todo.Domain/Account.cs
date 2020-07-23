using System;
using System.Collections.Generic;
using System.Text;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class Account : Entity
    {
        public Account()
        {
            
        }
        public Account(Guid accountId, string email, int planId)
        {
            Id = accountId;
            Email = email;
            PlanId = planId;

            DomainEvents.Add(new AccountCreated() { AccountId = Id, PlanId = PlanId });   
        }    

        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public string Email { get; set; }
        public int PlanId { get; set; }
        public string PaymentId {get; set;}
    }
}
