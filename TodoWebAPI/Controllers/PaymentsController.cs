using System;
using TodoWebAPI.BraintreeService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Braintree;
using TodoWebAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using TodoWebAPI.UserStories.RoleChanges;
using Todo.Domain.Repositories;
using Todo.Infrastructure.PaymentMethods;
using Todo.Infrastructure;
using TodoWebAPI.UserStories.DeletePaymentMethod;
using TodoWebAPI.UserStories;

namespace TodoWebAPI.Controllers
{
    [Authorize]
    [ApiController, Route("api/[controller]"), Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IMediator _mediator;
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethod;

        public PaymentsController(
            IBraintreeConfiguration braintreeConfiguration,
            IMediator mediator,
            IAccountRepository accountRepository,
            IPaymentMethodRepository paymentMethod
            )
        {
            _braintreeConfiguration = braintreeConfiguration;
            _mediator = mediator;
            _accountRepository = accountRepository;
            _paymentMethod = paymentMethod;
        }

        public static readonly TransactionStatus[] transactionSuccessStatuses =
            {
            TransactionStatus.AUTHORIZED,
            TransactionStatus.AUTHORIZING,
            TransactionStatus.SETTLED,
            TransactionStatus.SETTLING,
            TransactionStatus.SETTLEMENT_CONFIRMED,
            TransactionStatus.SETTLEMENT_PENDING,
            TransactionStatus.SUBMITTED_FOR_SETTLEMENT
        };

        [HttpGet, Route("generatetoken")]
        public object GenerateToken()
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            return clientToken;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethod()
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var gateway = _braintreeConfiguration.GetGateway();
            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(accountId);

            if (paymentMethod != null)
            {
                CreditCard creditCard = (CreditCard)await gateway.PaymentMethod.FindAsync(paymentMethod.TokenId);

                var creditCardInfo = new CardInfoModel()
                {
                    CardType = creditCard.CardType.ToString(),
                    ExpirationDate = creditCard.ExpirationDate,
                    LastFourDigits = creditCard.LastFour
                };

                return Ok(creditCardInfo);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<bool> AddPaymentMethod([FromBody] AddPaymentMethodModel addPaymentModel)
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var account = await _accountRepository.FindAccountByIdAsync(accountId);
            var gateway = _braintreeConfiguration.GetGateway();
            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(accountId);

            if (paymentMethod != null)
            {
                var deletePayment = new DeletePaymentMethod
                {
                    Method = paymentMethod
                };
                await _mediator.Send(deletePayment);
            }

            Customer customer = await gateway.Customer.FindAsync(account.PaymentId);

            var request = new PaymentMethodRequest()
            {
                CustomerId = customer.Id,
                PaymentMethodNonce = addPaymentModel.PaymentMethodNonce,
                Token = Guid.NewGuid().ToString()
            };

            Result<PaymentMethod> result = await gateway.PaymentMethod.CreateAsync(request);

            var payment = new Payment()
            {
                AccountId = accountId,
                TokenId = request.Token
            };

            _paymentMethod.Add(payment);

            await _paymentMethod.SaveChangesAsync();

            return result.IsSuccess();
        }

        [HttpPost, Route("subscription")]
        public async Task<IActionResult> CreatePaymentSubscription([FromBody] CreateSubscriptionModel createSubscriptionModel)
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var gateway = _braintreeConfiguration.GetGateway();
            var paymentMethod = await _paymentMethod.FindByAccountIdAsync(accountId);

            if (paymentMethod != null)
            {
                var subscription = new CreateSubscription
                {
                    AccountId = accountId,
                    Plan = createSubscriptionModel.PlanName
                };

                var response = await _mediator.Send(subscription);

                if (response == true)
                {
                    var planChange = new PlanChange
                    {
                        AccountId = accountId,
                        Plan = createSubscriptionModel.PlanName
                    };
                    await _mediator.Send(planChange);

                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost, Route("change")]
        public async Task<IActionResult> ChangePaymentSubscription([FromBody] ChangePaymentSubscription changePayment)
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var account = await _accountRepository.FindAccountByIdAsync(accountId);

            changePayment.AccountId = accountId;

            var planChange = new PlanChange
            {
                AccountId = accountId,
                Plan = changePayment.Plan
            };

            var response = await _mediator.Send(planChange);

            if (response == true)
            {
                var brainTreeResponse = await _mediator.Send(changePayment);
                if (brainTreeResponse == true)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
