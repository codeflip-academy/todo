using System;
using Todo.Domain;

namespace Todo.Infrastructure
{
    public class Payment : Entity
    {
        public string TokenId {get; set;}
        public Guid AccountId {get; set;}
    }
}

