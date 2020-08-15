using System;

namespace Todo.Infrastructure.Dto
{
    public class PlanDto
    {
        public string Name { get; set; }
        public int MaxContributors { get; set; }
        public int MaxLists { get; set; }
        public bool CanAddDueDates { get; set; }
    }
}