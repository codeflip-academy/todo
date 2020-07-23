using System;
using System.Collections.Generic;

namespace Todo.Domain
{
    public class AccountPlanAuthorizationValidator
    {
        public AccountPlan AccountPlan { get; private set; }
        public Plan Plan { get; private set; }
        public AccountPlanAuthorizationValidator(AccountPlan accountPlan, Plan plan)
        {
            AccountPlan = accountPlan;
            Plan = plan;
        }
        public bool CanCreateList() => AccountPlan.ListCount < Plan.MaxLists || Plan.MaxLists == -1;
        public bool CanAddContributor(TodoList list) => list.GetContributorCountExcludingOwner() < Plan.MaxContributors || Plan.MaxContributors == -1;
        public bool CanAddDueDate() => Plan.CanAddDueDates;
    }
}