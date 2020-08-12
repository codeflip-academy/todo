using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class PlanChangeUserStory : IRequestHandler<PlanChange, bool>
    {
        private readonly IAccountPlanRepository _accountPlan;

        public PlanChangeUserStory(IAccountPlanRepository accountPlan)
        {
            _accountPlan = accountPlan;
        }

        public async Task<bool> Handle(PlanChange request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(request.AccountId);

            if (request.Plan == "Free")
            {
                if (accountPlan.IsNewPlanLessThanCurrentPlan(PlanTiers.Free) == true)
                {
                    if (accountPlan.ListCount > PlanTiers.FreeMaxLists)
                        return false;
                }
                accountPlan.PlanId = PlanTiers.Free;
            }
            else if (request.Plan == "Basic")
            {
                if (accountPlan.IsNewPlanLessThanCurrentPlan(PlanTiers.Basic) == true)
                {
                    if (accountPlan.ListCount > PlanTiers.BasisMaxLists)
                        return false;
                }
                accountPlan.PlanId = PlanTiers.Basic;
            }
            else if (request.Plan == "Premium")
            {
                accountPlan.PlanId = PlanTiers.Premium;
            }

            await _accountPlan.SaveChangesAsync();

            return true;
        }
    }
}