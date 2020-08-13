using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class AddPayment : IRequest<bool>
    {
        public string PaymentMethodNonce { get; set; }
        public Guid AccountId { get; set; }
    }
}