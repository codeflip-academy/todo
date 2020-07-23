using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class RemoveContributorFromList : IRequest
    {
        public Guid ListId {get; set;}
        public Guid AccountId {get; set;}
        public string Email {get; set;}
    }
}