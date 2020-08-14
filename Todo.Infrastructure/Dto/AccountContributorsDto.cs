using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Infrastructure.Dto
{
    public class AccountContributorsDto
    {
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public string Email { get; set; }
    }
}
