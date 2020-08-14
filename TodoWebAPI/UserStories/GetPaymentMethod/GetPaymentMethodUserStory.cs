using System;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories
{
    public class GetPaymentMethodUserStory : IRequestHandler<GetPaymentMethod, CardInfoDto>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IAccountRepository _account;
        private readonly IPaymentMethodRepository _paymentMethod;

        public GetPaymentMethodUserStory(IBraintreeConfiguration braintreeConfiguration, IAccountRepository account)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _account = account;
        }

        public async Task<CardInfoDto> Handle(GetPaymentMethod request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var account = await _account.FindAccountByIdAsync(request.AccountId);

            if (account.PaymentMethodId != null)
            {
                CreditCard creditCard = (CreditCard)await gateway.PaymentMethod.FindAsync(account.PaymentMethodId);

                var creditCardInfo = new CardInfoDto()
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