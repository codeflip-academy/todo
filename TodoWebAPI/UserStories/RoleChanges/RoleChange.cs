using System;
using MediatR;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class RoleChange : IRequest
    {
        public string Plan {get; set;}
        public Guid AcountId {get; set;}
    }
}