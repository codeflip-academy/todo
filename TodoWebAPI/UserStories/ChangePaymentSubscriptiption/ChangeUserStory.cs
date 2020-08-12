using System;
using System.Linq;
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
    public class ChangePaymentSubscriptionUserStory : IRequestHandler<ChangePaymentSubscription, bool>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;
        private readonly IAccountRepository _account;

        public ChangePaymentSubscriptionUserStory(IBraintreeConfiguration braintreeConfiguration,
         IPaymentMethodRepository paymentMethod,
          IAccountRepository account)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
            _account = account;
        }

        public async Task<bool> Handle(ChangePaymentSubscription request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(request.AccountId);
            var account = await _account.FindAccountByIdAsync(request.AccountId);
            var subscriptionId = Guid.NewGuid().ToString();
            var plans = gateway.Plan.All();

            var plan = (from p in plans where p.Name == request.Plan select p).FirstOrDefault();

            if (paymentMethod != null)
            {
                var brainPlanValue = SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan);

                Subscription subscription = gateway.Subscription.Find(account.SubscriptionId);

                var result = gateway.Subscription.Update(subscription.Id, new SubscriptionRequest
                {
                    Id = subscriptionId,
                    PaymentMethodToken = paymentMethod.TokenId,
                    PlanId = brainPlanValue,
                    Price = plan.Price
                });

                account.SubscriptionId = subscriptionId;

                if (result.IsSuccess())
                {
                    await _account.SaveChangesAsync();

                    return true;
                }

            }
            return false;
        }
    }
}