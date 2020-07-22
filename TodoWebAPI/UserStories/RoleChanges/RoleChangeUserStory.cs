using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class RoleChangeUserStory : AsyncRequestHandler<RoleChange>
    {
        private readonly IAccountPlanRepository _accountPlanRepository;

        public RoleChangeUserStory(IAccountPlanRepository accountPlanRepository)
        {
            _accountPlanRepository = accountPlanRepository;
        }
        protected override async Task Handle(RoleChange request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AcountId);

            if (request.Plan == "Free")
            {
                accountPlan.PlanId = PlanTiers.Free;
            }
            else if (request.Plan == "Basic")
            {
                accountPlan.PlanId = PlanTiers.Basic;
            }
            else if (request.Plan == "Premium")
            {
                accountPlan.PlanId = PlanTiers.Premium;
            }

            await _accountPlanRepository.SaveChangesAsync();
        }
    }
}