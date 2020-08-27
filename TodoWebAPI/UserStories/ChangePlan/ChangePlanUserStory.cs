using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class ChangePlanUserStory : IRequestHandler<ChangePlan, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IDowngradeRepository _downgradeRepository;
        private readonly IBraintreeConfiguration _braintreeConfiguration;

        public ChangePlanUserStory(
            IAccountRepository accountRepository,
            IAccountPlanRepository accountPlan,
            IDowngradeRepository downgradeRepository,
            IBraintreeConfiguration braintreeConfiguration)
        {
            _accountRepository = accountRepository;
            _accountPlan = accountPlan;
            _downgradeRepository = downgradeRepository;
            _braintreeConfiguration = braintreeConfiguration;
        }

        public async Task<bool> Handle(ChangePlan request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindAccountByIdAsync(request.AccountId);
            var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(request.AccountId);
            var gateway = _braintreeConfiguration.GetGateway();
            var currentSubscription = await gateway.Subscription.FindAsync(account.SubscriptionId);
            var planId = PlanTiers.ConvertPlanNameToInt(request.Plan);
            var downgrading = accountPlan.IsNewPlanLessThanCurrentPlan(planId);

            if (downgrading)
            {
                var downgrade = await _downgradeRepository.GetDowngradeByAccountIdAsync(account.Id);

                if (downgrade != null)
                {
                    downgrade.BillingCycleEnd = currentSubscription.BillingPeriodEndDate.GetValueOrDefault();
                    downgrade.PlanId = planId;
                }
                else
                {
                    await _downgradeRepository.Add(
                        accountId: request.AccountId,
                        billingCycleEnd: currentSubscription.BillingPeriodEndDate.GetValueOrDefault(),
                        planId: planId);
                }
            }
            else
            {
                accountPlan.ChangePlan(planId);
            }

            await _accountPlan.SaveChangesAsync();

            return true;
        }
    }
}