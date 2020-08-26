using System;
using MediatR;

namespace TodoWebAPI.UserStories.EmailFilter
{
    public class EmailFilter : IRequest
    {
        public bool EmailDueDate { get; set; }
        public bool EmailCompleted { get; set; }
        public Guid AccountId { get; set; }
    }
}