using System;

namespace TodoWebAPI
{
    public static class DiscountCalculator
    {
        public static decimal CalculateDiscount(decimal price, int percentage)
        {
            var decimalPercent = percentage / 100M;
            var discount = price * decimalPercent;

            return discount;
        }
    }
}