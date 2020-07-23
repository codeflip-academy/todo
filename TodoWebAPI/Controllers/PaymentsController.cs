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

namespace TodoWebAPI.Controllers
{
    [Authorize]
    [ApiController, Route("api/[controller]"), Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IMediator _mediator;

        public PaymentsController(IBraintreeConfiguration braintreeConfiguration, IMediator mediator)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _mediator = mediator;
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

        [HttpGet, Route("GenerateToken")]
        public object GenerateToken()
        {
            var gateway = _braintreeConfiguration.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            return clientToken;
        }

        [HttpPost, Route("Checkout")]
        public async Task<bool> Checkout(VmCheckout model)
        {
            var gateway = _braintreeConfiguration.GetGateway();

            var customerRequest = new CustomerRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PaymentMethodNonce = model.PaymentMethodNonce
            };
            
            Result<Customer> customerResult = await gateway.Customer.CreateAsync(customerRequest);
            bool success = customerResult.IsSuccess();
            Customer customer = customerResult.Target;
            string customerId = customer.Id;
            string cardToken = customer.PaymentMethods[0].Token;

            var request = new SubscriptionRequest
            {
                PaymentMethodToken = cardToken,
                PaymentMethodNonce = model.PaymentMethodNonce,
                PlanId = "2",
            };

            var result = await gateway.Subscription.CreateAsync(request);

            if (result.IsSuccess())
            {
                var plan = new RoleChange()
                {
                    Plan = "Basic",
                    AcountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value)
                };

                await _mediator.Send(plan);

                return true;
            }

            return false;
        }
    }
}
