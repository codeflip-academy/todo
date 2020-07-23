using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Todo.Domain;

namespace TodoWebAPI.Models
{
    public class CreateItemViewModel
    {
        public Guid ListId { get; set; }
        [StringLength(200)]
        public string Notes { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
