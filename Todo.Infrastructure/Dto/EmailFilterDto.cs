using System;

namespace Todo.Infrastructure.Dto
{
    public class EmailFilterDto
    {
        public bool EmailDueDate { get; set; }
        public bool EmailListCompleted { get; set; }
        public bool EmailItemCompleted { get; set; }
        public bool EmailInvitation { get; set; }
    }
}