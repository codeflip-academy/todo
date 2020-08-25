using System;
using MediatR;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories.DeletePaymentMethod
{
    public class DeletePaymentMethod : IRequest
    {
        public string PaymentMethodId { get; set; }
        public Guid AccountId { get; set; }
        public string Plan { get; set; }
    }
}