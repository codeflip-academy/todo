using System;

namespace Todo.Domain
{
    public class Plan : Entity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int MaxContributors { get; private set; }
        public int MaxLists { get; private set; }
        public bool CanNotifyViaEmail { get; private set; }
        public bool CanAddDueDates { get; private set; }
    }
}