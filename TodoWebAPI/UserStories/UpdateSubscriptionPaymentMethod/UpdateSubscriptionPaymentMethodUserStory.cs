using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain.Repositories;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories
{
    public class UpdateSubscriptionPaymentMethodUserStory : AsyncRequestHandler<UpdateSubscriptionPaymentMethod>
    {
        private readonly IAccountRepository _account;
        private readonly IBraintreeConfiguration _braintreeConfiguration;

        public UpdateSubscriptionPaymentMethodUserStory(IAccountRepository account, IBraintreeConfiguration braintreeConfiguration)
        {
            _account = account;
            _braintreeConfiguration = braintreeConfiguration;
        }

        protected async override Task Handle(UpdateSubscriptionPaymentMethod request, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(request.AccountId);
            var gateway = _braintreeConfiguration.GetGateway();
            var subscriptionId = Guid.NewGuid().ToString();

            var subscriptionUpdated = await gateway.Subscription.UpdateAsync(account.SubscriptionId, new SubscriptionRequest
            {
                Id = subscriptionId,
                PaymentMethodToken = account.PaymentMethodId,
            });

            account.SubscriptionId = subscriptionId;

            await _account.SaveChangesAsync();
        }
    }
}