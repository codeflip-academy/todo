using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TodoWebAPI.ViewModels
{
    public class RedeemCouponViewModel
    {
        [FromBody]
        [Required]
        [StringLength(6)]
        public string CouponCode { get; set; }
    }
}