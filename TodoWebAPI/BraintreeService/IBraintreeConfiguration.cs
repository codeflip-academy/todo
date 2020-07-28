using Braintree;
using System;
using System.Collections.Generic;
using System.Text;

namespace TodoWebAPI.BraintreeService
{
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    };
}
