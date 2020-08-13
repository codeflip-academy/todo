using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories
{
    public class GetPaymentMethodUserStory : IRequestHandler<GetPaymentMethod, CardInfoModel>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;

        public GetPaymentMethodUserStory(IBraintreeConfiguration braintreeConfiguration, IPaymentMethodRepository paymentMethod)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
        }

        public async Task<CardInfoModel> Handle(GetPaymentMethod request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(request.AccountId);

            if (paymentMethod != null)
            {
                CreditCard creditCard = (CreditCard)await gateway.PaymentMethod.FindAsync(paymentMethod.TokenId);

                var creditCardInfo = new CardInfoModel()
                {
                    CardType = creditCard.CardType.ToString(),
                    ExpirationDate = creditCard.ExpirationDate,
                    LastFourDigits = creditCard.LastFour
                };

                return (creditCardInfo);
            }
            return null;
        }
    }
}