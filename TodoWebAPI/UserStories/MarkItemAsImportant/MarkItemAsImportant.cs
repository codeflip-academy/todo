using System;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class MarkItemAsImportant : IRequest
    {
        public Guid ItemId { get; set; }
        public bool Important { get; set; }
    }
}