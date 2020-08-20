using System;
using Todo.Core;

namespace Todo.Domain
{
    public class Coupon : Entity
    {
        public int Id { get; set; }
        public string CouponValue { get; set; }
    }
}