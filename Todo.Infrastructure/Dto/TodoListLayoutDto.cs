using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Domain;
using TodoWebAPI.Data;

namespace Todo.Infrastructure.Dto
{
    public class TodoListLayoutDto
    {
        public Guid Id { get; set; }
        public Guid ListId { get; set; }
        public string Layout { get; set; }
    }
}
