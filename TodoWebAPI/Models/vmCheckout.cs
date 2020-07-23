using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoWebAPI.Models
{
    public class VmCheckout
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public decimal Price { get; set; }
        public string PaymentMethodNonce { get; set; }
        public string PlanName { get; set; }
    }
}
