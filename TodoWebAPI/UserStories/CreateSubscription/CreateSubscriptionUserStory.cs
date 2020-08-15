using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain.Repositories;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories
{
    public class CreateSubscriptionUserStory : IRequestHandler<CreateSubscription, bool>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;
        private readonly IAccountRepository _account;

        public CreateSubscriptionUserStory(IBraintreeConfiguration braintreeConfiguration, IPaymentMethodRepository paymentMethod, IAccountRepository account)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
            _account = account;
        }

        public async Task<bool> Handle(CreateSubscription request, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(request.AccountId);

            if (account.SubscriptionId != null)
            {
                return true;
            }

            var gateway = _braintreeConfiguration.GetGateway();

            var brainType = SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan);

            var re = new SubscriptionRequest
            {
                PaymentMethodToken = account.PaymentMethodId,
                PlanId = brainType,
                Id = Guid.NewGuid().ToString()
            };

            account.SubscriptionId = re.Id;

            await _account.SaveChangesAsync();

            Result<Subscription> result = gateway.Subscription.Create(re);
            if (result.IsSuccess())
                return true;

            return false;
        }
    }
}