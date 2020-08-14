using System;
using System.Collections.Generic;
using Todo.Domain;

namespace Todo.Infrastructure.Dto
{
    public class TodoListsDto
    {
        public List<TodoListDto> TodoLists { get; set; }
        public Dictionary<string, AccountContributorsDto> Contributors { get; set; }
        public TodoListsDto(List<TodoListDto> todoLists, Dictionary<string, AccountContributorsDto> contributors)
        {
            TodoLists = todoLists;
            Contributors = contributors;
        }
    }
}
