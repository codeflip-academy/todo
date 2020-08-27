using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories
{
    public class UpdateSubscriptionPaymentMethodUserStory : AsyncRequestHandler<UpdateSubscriptionPaymentMethod>
    {
        private readonly IAccountRepository _account;
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IDowngradeRepository _downgradeRepository;

        public UpdateSubscriptionPaymentMethodUserStory(IAccountRepository account,
        IBraintreeConfiguration braintreeConfiguration,
         IAccountPlanRepository accountPlan,
         IDowngradeRepository downgradeRepository)
        {
            _account = account;
            _braintreeConfiguration = braintreeConfiguration;
            _accountPlan = accountPlan;
            _downgradeRepository = downgradeRepository;
        }

        protected async override Task Handle(UpdateSubscriptionPaymentMethod request, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(request.AccountId);
            var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(request.AccountId);
            var gateway = _braintreeConfiguration.GetGateway();
            var subscriptionId = Guid.NewGuid().ToString();

            if (account.HasPaymentMethodDeletedPlan())
            {
                var downgrade = await _downgradeRepository.GetDowngradeByAccountIdAsync(request.AccountId);

                var createSubscriptionRequest = new SubscriptionRequest
                {
                    PaymentMethodToken = account.PaymentMethodId,
                    PlanId = SubscriptionHelper.ConvertPlanToBrainTreeType(account.PaymentMethodDeletedPlan),
                    Id = subscriptionId,
                };

                var createSubscriptionResult = gateway.Subscription.Create(createSubscriptionRequest);

                account.UpdateSubscriptionId(createSubscriptionRequest.Id);

                account.RemovePaymentMethodDeletedPlan();

                await _downgradeRepository.Remove(downgrade);

                await _account.SaveChangesAsync();
            }
            else
            {
                var subscriptionUpdatedResult = await gateway.Subscription.UpdateAsync(account.SubscriptionId, new SubscriptionRequest
                {
                    PaymentMethodToken = account.PaymentMethodId,
                });
            }
        }
    }
}