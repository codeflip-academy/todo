using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Domain;

namespace Todo.Domain
{
    public class InvitationAccepted : INotification
    {
        public Guid SenderAccountId { get; set; }
        public TodoList List { get; set; }
        public string Email { get; set; }
    }
}
