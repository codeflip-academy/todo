using System;

namespace TodoWebAPI.Presentation
{
    public class PlanPresentation
    {
        public string Name { get; set; }
        public int MaxContributors { get; set; }
        public int MaxLists { get; set; }
        public bool CanAddDueDates { get; set; }
    }
}