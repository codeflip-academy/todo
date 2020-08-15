using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Infrastructure.Dto
{
    public class CreateListDto
    {
        public Guid Id { get; set; }
        public string ListTitle { get; set; }
    }
}
