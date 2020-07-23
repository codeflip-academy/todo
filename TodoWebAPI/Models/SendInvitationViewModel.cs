using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TodoWebAPI.Models
{
    public class SendInvitationViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email Required")]
        [FromBody]
        public string Email { get; set; }
    }
}
