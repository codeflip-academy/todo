using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Domain;
using TodoWebAPI.Data;

namespace Todo.Infrastructure.Dto
{
    public class TodoItemLayoutDto
    {
        public List<string> Layout { get; set; }
    }
}
