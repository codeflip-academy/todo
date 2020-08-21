using System;
using Todo.Core;

namespace Todo.Domain
{
    public class Discount : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Percentage { get; set; }
        public byte BillingCycles { get; set; }
    }
}