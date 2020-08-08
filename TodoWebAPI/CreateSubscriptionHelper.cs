using System;

namespace TodoWebAPI.UserStories
{
    public static class SubscriptionHelper
    {
        public static string ConvertPlanToBrainTreeType(string stringPlan)
        {
            var intPlan = "";
            if (stringPlan == "Free")
            {
                intPlan = "1";
            }
            else if (stringPlan == "Basic")
            {
                intPlan = "2";
            }
            else if (stringPlan == "Premium")
            {
                intPlan = "3";
            }

            return intPlan;
        }
    }
}