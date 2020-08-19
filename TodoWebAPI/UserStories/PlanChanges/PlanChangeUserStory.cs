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
    public class PlanChangeUserStory : IRequestHandler<ChangePlan, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IDowngradeRepository _downgradeRepository;
        private readonly IBraintreeConfiguration _braintreeConfiguration;

        public PlanChangeUserStory(
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

            if (request.Plan == "Free")
            {
                if (accountPlan.IsNewPlanLessThanCurrentPlan(PlanTiers.Free) == true)
                {
                    var downgrade = await _downgradeRepository.GetDowngradeByAccountIdAsync(account.Id);

                    if (downgrade != null)
                    {
                        downgrade.AccountId = request.AccountId;
                        downgrade.BillingCycleEnd = currentSubscription.BillingPeriodEndDate.GetValueOrDefault();
                        downgrade.PlanId = Int32.Parse(SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan));
                    }
                    else
                    {
                        await _downgradeRepository.Add(
                            accountId: request.AccountId,
                            billingCycleEnd: currentSubscription.BillingPeriodEndDate.GetValueOrDefault(),
                            planId: Int32.Parse(SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan)));
                    }

                }
                else
                {
                    accountPlan.ChangePlan(PlanTiers.Free);
                }
            }
            else if (request.Plan == "Basic")
            {
                if (accountPlan.IsNewPlanLessThanCurrentPlan(PlanTiers.Basic) == true)
                {
                    var downgrade = await _downgradeRepository.GetDowngradeByAccountIdAsync(account.Id);

                    if (downgrade != null)
                    {
                        downgrade.AccountId = request.AccountId;
                        downgrade.BillingCycleEnd = currentSubscription.BillingPeriodEndDate.GetValueOrDefault();
                        downgrade.PlanId = Int32.Parse(SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan));
                    }
                    else
                    {
                        await _downgradeRepository.Add(
                            accountId: request.AccountId,
                            billingCycleEnd: currentSubscription.BillingPeriodEndDate.GetValueOrDefault(),
                            planId: Int32.Parse(SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan)));
                    }
                }
                else
                {
                    accountPlan.ChangePlan(PlanTiers.Basic);
                }
            }
            else if (request.Plan == "Premium")
            {
                accountPlan.ChangePlan(PlanTiers.Premium);
            }

            await _accountPlan.SaveChangesAsync();

            return true;
        }
    }
}