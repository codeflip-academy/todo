using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Todo.Infrastructure.Dto
{
    public class TodoListDto
    {
        public Guid Id { get; set; }
        public string ListTitle { get; set; }
        public bool Completed { get; set; }
        public List<string> Contributors { get; set; }
        public byte Role { get; set; }
        public int IncompleteCount { get; set; }
    }
}
