using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;
using TodoWebAPI.UserStories.RoleChanges;

namespace TodoWebAPI.UserStories
{
    public class DowngradePaymentSubscriptionUserStory : IRequestHandler<DowngradePaymentSubscription, bool>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;
        private readonly IAccountRepository _account;

        public DowngradePaymentSubscriptionUserStory(IBraintreeConfiguration braintreeConfiguration, IPaymentMethodRepository paymentMethod, IAccountRepository account)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
            _account = account;
        }

        public async Task<bool> Handle(DowngradePaymentSubscription request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(request.AccountId);
            var account = await _account.FindAccountByIdAsync(request.AccountId);

            if (paymentMethod != null)
            {
                var brainPlanValue = CreateSubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan);

                Subscription subscription = gateway.Subscription.Find(account.SubscriptionId);

                var result = gateway.Subscription.Update(subscription.Id, new SubscriptionRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    PaymentMethodNonce = paymentMethod.TokenId,
                    PlanId = brainPlanValue
                });

                if (result.IsSuccess())
                    return true;
            }
            return false;
        }
    }
}