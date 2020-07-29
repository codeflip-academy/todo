using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class CreateSubscription : IRequest<bool>
    {
        public Guid AccountId {get; set;}
        public string Plan {get; set;}
    }
}