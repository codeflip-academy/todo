using System;
using MediatR;
using Todo.Domain;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class RoleChange : IRequest<AccountPlan>
    {
        public string Plan {get; set;}
        public Guid AcountId {get; set;}
    }
}