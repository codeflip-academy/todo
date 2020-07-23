using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoWebAPI.UserStories.EditItem
{
    public class EditItem : IRequest
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; }
        public Guid ListId { get; set; }
        public Guid ItemId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Notes { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
