using System;

namespace Todo.Infrastructure.Dto
{
    public class DowngradeDto
    {
        public Guid AccountId { get; set; }
        public DateTime BiliingCycleEnd { get; set; }
        public int PlanId { get; set; }
    }
}