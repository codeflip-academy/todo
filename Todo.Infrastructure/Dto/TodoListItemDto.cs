using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Domain;
using TodoWebAPI.Data;

namespace Todo.Infrastructure.Dto
{
    public class TodoListItemDto
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public bool Completed { get; protected set; }
        public string Name { get; set; }
        public Guid? ListId { get; set; }
        public DateTime? DueDate { get; set; }
        public bool HasSubItems { get; set; }
        public bool Important { get; set; }
    }
}
