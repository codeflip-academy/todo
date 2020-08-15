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

            var payment = new GetPaymentMethod
            {
                AccountId = accountId
            };

            var result = await _mediator.Send(payment);

            if (result != null)
                return Ok(result);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrReplacePaymentMethod([FromBody] AddPayment addPayment)
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var account = await _accountRepository.FindAccountByIdAsync(accountId);
            var previousPaymentMethodId = account.PaymentMethodId;
            var gateway = _braintreeConfiguration.GetGateway();
            var subscriptionId = Guid.NewGuid().ToString();

            addPayment.AccountId = accountId;

            var paymentMethodAdded = await _mediator.Send(addPayment);

            if (account.SubscriptionId != null)
            {
                var updateSubscription = new UpdateSubscriptionPaymentMethod
                {
                    AccountId = accountId
                };

                await _mediator.Send(updateSubscription);
            }

            if (previousPaymentMethodId != null)
            {
                var deletePayment = new DeletePaymentMethod
                {
                    PaymentMethodId = previousPaymentMethodId,
                    AccountId = accountId
                };
                await _mediator.Send(deletePayment);
            }

            if (paymentMethodAdded)
                return Ok();

            return BadRequest();
        }

        [HttpPost, Route("subscription/change")]
        public async Task<IActionResult> ChangePaymentSubscription([FromBody] ChangePaymentSubscription changePayment)
        {
            changePayment.AccountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var gateway = _braintreeConfiguration.GetGateway();

            var createSubscription = new CreateSubscription
            {
                AccountId = changePayment.AccountId,
                Plan = changePayment.Plan
            };

            var result = await _mediator.Send(createSubscription);

            if (result)
            {
                var planChange = new ChangePlan
                {
                    AccountId = changePayment.AccountId,
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
                else
                {
                    return Forbid();
                }
            }

            return BadRequest();
        }

        [HttpDelete, Route("paymentMethod/delete")]
        public async Task<IActionResult> DeletePayment(DeletePaymentMethod deletePaymentMethod)
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);
            var account = await _accountRepository.FindAccountByIdAsync(accountId);
            deletePaymentMethod.PaymentMethodId = account.PaymentMethodId;
            deletePaymentMethod.AccountId = accountId;

            await _mediator.Send(deletePaymentMethod);

            return Ok();
        }
    }
}
