using System;

namespace CodeJar.Response
{
    public class RedeemCodeResponse
    {
        private bool SuccessStatus { get; set; }
        public string PromotionType { get; private set; }
        internal void SetSuccessStatus(bool successStatus)
        {
            SuccessStatus = successStatus;
        }
        internal void SetPromotionType(string promotionType)
        {
            PromotionType = promotionType;
        }

        public bool IsSuccess()
        {
            return SuccessStatus;
        }
    }
}