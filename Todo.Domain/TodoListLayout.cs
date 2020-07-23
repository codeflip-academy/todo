using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class TodoListLayout : Entity
    {
        public Guid Id { get; set; }
        public Guid AccountId {get; set;}
        public Guid ListId { get; set; }
        public List<Guid> Layout { get; set; } = new List<Guid>();
        public void UpdateLayout(Guid todoListItemId,  int todoListItemPosition, Guid listId)
        {
            Layout.Remove(todoListItemId);
            Layout.Insert(todoListItemPosition, todoListItemId);

            if (Layout.Distinct().Count() != Layout.Count)
            {
                throw new ArgumentException("Layout contains duplicate Id.");
            }
            else
            {
                DomainEvents.Add(new ListLayoutUpdated { Position = todoListItemPosition, ItemId = todoListItemId, ListId = listId, AccountId = AccountId });
            }
        }

        public void RemoveItemFromLayout(Guid todoListItemId)
        {
            Layout.Remove(todoListItemId);
        }
    }
}
