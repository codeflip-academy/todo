using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class DowngradePaymentSubscription : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public string Plan { get; set; }
    }
}