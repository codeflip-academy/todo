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
        private readonly IAccountRepository _accountRepository;
        private readonly IPlanRepository _plan;

        public RoleChangeUserStory(IAccountRepository accountRepository, IPlanRepository plan)
        {
            _accountRepository = accountRepository;
            _plan = plan;
        }
        protected override async Task Handle(RoleChange request, CancellationToken cancellationToken)
        {
            var account =  await _accountRepository.FindAccountByIdAsync(request.AcountId);
            var plan = await _plan.FindPlanByIdAsync(account.PlanId);

            if(request.Plan == "Free")
            {
                account.PlanId = PlanTiers.Free;
            }
            else if(request.Plan == "Basic")
            {
                account.PlanId = PlanTiers.Basic;
            }
            else if(request.Plan == "Premium")
            {
                account.PlanId = PlanTiers.Premium;
            }

            _accountRepository.UpdateAccountPlan(account);
            await _accountRepository.SaveChangesAsync();
        }
    }
}