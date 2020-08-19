using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Todo.Core;
using Todo.Domain;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class AccountPlan : Entity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public int PlanId { get; private set; }
        public int ListCount { get; private set; }
        public void IncrementListCount() => ListCount++;
        public void DecrementListCount() => ListCount--;
        public bool IsNewPlanLessThanCurrentPlan(int planId)
        {
            if (planId < PlanId)
            {
                return true;
            }
            return false;
        }
        public void ChangePlan(int planId)
        {
            PlanId = planId;
        }
    }
}
