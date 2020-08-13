using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEvents
{
    public class EditSubItem : INotification
    {
        public SubItem SubItem { get; set; }
    }
}
