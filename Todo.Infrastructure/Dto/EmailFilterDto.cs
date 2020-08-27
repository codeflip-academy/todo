using System;

namespace Todo.Infrastructure.Dto
{
    public class EmailFilterDto
    {
        public bool EmailDueDate { get; set; }
        public bool EmailCompleted { get; set; }
    }
}