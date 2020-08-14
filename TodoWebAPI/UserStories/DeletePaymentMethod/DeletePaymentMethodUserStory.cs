using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain.Repositories;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories.DeletePaymentMethod
{
    public class DeletePaymentMethodUserStory : AsyncRequestHandler<DeletePaymentMethod>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IAccountRepository _account;

        public DeletePaymentMethodUserStory(IBraintreeConfiguration braintreeConfiguration, IAccountRepository account)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _account = account;
        }
        protected async override Task Handle(DeletePaymentMethod request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();

            var result = gateway.PaymentMethod.Delete(request.PaymentMethodId);
        }
    }
}