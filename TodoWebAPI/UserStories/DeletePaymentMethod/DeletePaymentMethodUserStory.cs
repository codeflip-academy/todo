using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories.DeletePaymentMethod
{
    public class DeletePaymentMethodUserStory : AsyncRequestHandler<DeletePaymentMethod>
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IBraintreeConfiguration _braintreeConfiguration;

        public DeletePaymentMethodUserStory(IPaymentMethodRepository repository, IBraintreeConfiguration braintreeConfiguration)
        {
            _repository = repository;
            _braintreeConfiguration = braintreeConfiguration;
        }
        protected async override Task Handle(DeletePaymentMethod request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();

            var result = gateway.PaymentMethod.Delete(request.Method.TokenId);
            
            _repository.Remove(request.Method);
            await _repository.SaveChangesAsync();
        }
    }
}