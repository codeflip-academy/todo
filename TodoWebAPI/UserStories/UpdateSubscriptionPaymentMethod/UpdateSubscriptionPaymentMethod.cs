using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class UpdateSubscriptionPaymentMethod : IRequest
    {
        public Guid AccountId { get; set; }
    }
}