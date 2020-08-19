using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class RedeemCoupon : IRequest<bool>
    {
        public string CouponCode { get; set; }
        public Guid AccountId { get; set; }
    }
}