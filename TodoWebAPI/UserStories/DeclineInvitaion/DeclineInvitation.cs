using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class DeclineInvitation : IRequest
    {
        public Guid AccountId {get; set;}
        public Guid ListId {get; set;}
    }
}