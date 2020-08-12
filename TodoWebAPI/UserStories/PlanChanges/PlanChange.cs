using System;
using MediatR;
using Todo.Domain;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class PlanChange : IRequest<bool>
    {
        public string Plan { get; set; }
        public Guid AccountId { get; set; }
    }
}