using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using Todo.Domain;

namespace TodoWebAPI.Models
{
    public class CreateAccountModel : IRequest<Account>
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        [Required(ErrorMessage = "Email Required")]
        [EmailAddress]
        public string Email { get; set; }
        public string PaymentId {get; set;}
    }
}
