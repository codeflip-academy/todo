using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories
{
    public class CreateSubscriptionUserStory : IRequestHandler<CreateSubscription, bool>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;

        public CreateSubscriptionUserStory(IBraintreeConfiguration braintreeConfiguration, IPaymentMethodRepository paymentMethod)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
        }

        public async Task<bool> Handle(CreateSubscription request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();

            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(request.AccountId);

            var brainType = CreateSubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan);

            var re = new SubscriptionRequest
            {
                PaymentMethodToken = paymentMethod.TokenId,
                PlanId = brainType
            };

            Result<Subscription> result = gateway.Subscription.Create(re);
            if (result.IsSuccess())
                return true;

            return false;
        }
    }
}