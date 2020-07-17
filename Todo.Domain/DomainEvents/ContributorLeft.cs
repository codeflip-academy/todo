using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Domain;

namespace Todo.Domain
{
    public class ContributorLeft : INotification
    {
        public Guid ListId { get; set; }
    }
}
