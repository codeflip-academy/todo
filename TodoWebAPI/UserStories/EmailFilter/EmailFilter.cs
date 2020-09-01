using System;
using MediatR;

namespace TodoWebAPI.UserStories.EmailFilter
{
    public class EmailFilter : IRequest
    {
        public bool EmailDueDate { get; set; }
        public bool EmailListCompleted { get; set; }
        public bool EmailItemCompleted { get; set; }
        public bool EmailInvitation { get; set; }
        public Guid AccountId { get; set; }
    }
}