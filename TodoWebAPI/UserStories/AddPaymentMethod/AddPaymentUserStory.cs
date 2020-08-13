using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;

namespace TodoWebAPI.UserStories
{
    public class AddPaymentUserStory : IRequestHandler<AddPayment, bool>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;
        private readonly IAccountRepository _account;

        public AddPaymentUserStory(IBraintreeConfiguration braintreeConfiguration, IPaymentMethodRepository paymentMethod, IAccountRepository account)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
            _account = account;
        }
        public async Task<bool> Handle(AddPayment request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var account = await _account.FindAccountByIdAsync(request.AccountId);

            Customer customer = await gateway.Customer.FindAsync(account.PaymentId);

            var result = new PaymentMethodRequest()
            {
                CustomerId = customer.Id,
                PaymentMethodNonce = request.PaymentMethodNonce,
                Token = Guid.NewGuid().ToString()
            };

            Result<PaymentMethod> response = await gateway.PaymentMethod.CreateAsync(result);

            var payment = new Payment()
            {
                AccountId = request.AccountId,
                TokenId = result.Token
            };

            _paymentMethod.Add(payment);

            if (response.IsSuccess())
            {
                await _paymentMethod.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}