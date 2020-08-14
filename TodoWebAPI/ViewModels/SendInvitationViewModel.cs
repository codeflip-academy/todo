using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TodoWebAPI.ViewModels
{
    public class SendInvitationViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email Required")]
        [FromBody]
        public string Email { get; set; }
    }
}
