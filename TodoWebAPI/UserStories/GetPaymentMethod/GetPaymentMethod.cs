using System;
using Braintree;
using MediatR;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories
{
    public class GetPaymentMethod : IRequest<CardInfoDto>
    {
        public Guid AccountId { get; set; }
    }
}