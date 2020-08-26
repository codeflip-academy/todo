using System;

namespace Todo.Domain
{
    public static class PlanTiers
    {
        public static readonly int Free = 1;
        public static readonly int Basic = 2;
        public static readonly int Premium = 3;
        public static readonly int FreeMaxLists = 5;
        public static readonly int BasisMaxLists = 10;

        public static int ConvertPlanNameToInt(string planName)
        {
            int plan = 0;

            switch (planName)
            {
                case "Free":
                    plan = 1;
                    break;
                case "Basic":
                    plan = 2;
                    break;
                case "Premium":
                    plan = 3;
                    break;
                default:
                    throw new ArgumentException("Plan name does not exist.");
            }

            return plan;
        }
    }
}