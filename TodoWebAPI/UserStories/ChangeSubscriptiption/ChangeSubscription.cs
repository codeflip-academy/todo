using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class ChangeSubscription : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        [Required]
        public string Plan { get; set; }
    }
}