using System;
using System.Collections.Generic;
using MediatR;
using Todo.Domain;

namespace TodoWebAPI.UserStories
{
    public class DowngradeDeleteLists : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public List<TodoList> NummberOfListsToDelete { get; set; }
    }
}