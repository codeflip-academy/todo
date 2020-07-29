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

            var intPlan = "";
            if (request.Plan == "Free")
            {
                intPlan = "1";
            }
            else if (request.Plan == "Basic")
            {
                intPlan = "2";
            }
            else if (request.Plan == "Premium")
            {
                intPlan = "3";
            }

            var re = new SubscriptionRequest
            {
                PaymentMethodToken = paymentMethod.TokenId,
                PlanId = intPlan
            };

            Result<Subscription> result = gateway.Subscription.Create(re);
            if (result.IsSuccess())
                return true;

            return false;
        }
    }
}