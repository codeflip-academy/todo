using System;
using Braintree;
using MediatR;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories
{
    public class GetPaymentMethod : IRequest<CardInfoModel>
    {
        public Guid AccountId { get; set; }
    }
}