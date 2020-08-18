using System;
using System.Collections.Generic;
using MediatR;
using Todo.Domain;

namespace TodoWebAPI.UserStories
{
    public class DowngradeContributorDeletion : IRequest
    {
        public Guid AccountId { get; set; }
        public int PlanId { get; set; }
    }
}