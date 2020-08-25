using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class GetUncompletedItemCount : IRequest<int>
    {
        public Guid ListId { get; set; }
    }
}
