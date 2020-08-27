using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories.DeletePaymentMethod
{
    public class DeletePaymentMethodUserStory : AsyncRequestHandler<DeletePaymentMethod>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IAccountRepository _account;
        private readonly IDowngradeRepository _downgradeRepository;

        public DeletePaymentMethodUserStory(IBraintreeConfiguration braintreeConfiguration,
         IAccountRepository account,
         IDowngradeRepository downgradeRepository)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _account = account;
            _downgradeRepository = downgradeRepository;
        }
        protected async override Task Handle(DeletePaymentMethod request, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(request.AccountId);
            var gateway = _braintreeConfiguration.GetGateway();
            var currentSubscription = await gateway.Subscription.FindAsync(account.SubscriptionId);
            var planId = PlanTiers.ConvertPlanNameToInt(request.Plan);

            if (request.Plan != "Free")
            {
                account.PaymentMethodDeletedPlan = request.Plan;
                await _downgradeRepository.Add(request.AccountId, currentSubscription.BillingPeriodEndDate.GetValueOrDefault(), planId);
            }

            account.RemovePaymentMethodId();

            gateway.PaymentMethod.Delete(request.PaymentMethodId);

            await _account.SaveChangesAsync();
        }
    }
}