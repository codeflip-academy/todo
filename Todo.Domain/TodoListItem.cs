using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class TodoListItem : Entity
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public bool Completed { get; protected set; }
        public string Name { get; set; }
        public Guid? ListId { get; set; }
        public DateTime? DueDate { get; set; }
        public void SetCompleted(List<SubItem> items)
        {
            if (!items.All(item => item.ListItemId == Id))
                return;

            var itemsCompleted = items.All(item => item.Completed);

            if (Completed && !itemsCompleted)
            {
                Completed = false;
                DomainEvents.Add(new TodoListItemCompletedStateChanged { Item = this });
            }
            else if (!Completed && itemsCompleted)
            {
                Completed = true;
                DomainEvents.Add(new TodoListItemCompletedStateChanged { Item = this });
            }
        }

        public void SetCompleted()
        {
            CheckIfListItemIsTrashed();
            
            if (Completed)
                return;

            Completed = true;
            DomainEvents.Add(new TodoListItemCompletedStateChanged { Item = this });
        }

        public void SetNotCompleted()
        {
            CheckIfListItemIsTrashed();

            if (!Completed)
                return;

            Completed = false;
            DomainEvents.Add(new TodoListItemCompletedStateChanged { Item = this });
        }

        public SubItem CreateSubItem(string name)
        {
            var item = new SubItem
            {
              ListItemId = this.Id,
              Name = name,
            };

            DomainEvents.Add(new SubItemCreated { SubItem = item });

            return item;
        }

        public void MoveToTrash()
        {
            var listId = this.ListId;
            this.ListId = null;

            DomainEvents.Add(new ItemMovedToTrash { ListId = listId, Item = this });
        }

        private void CheckIfListItemIsTrashed()
        {
            if(this.ListId == null)
                throw new InvalidOperationException("Item is in the trash!");
        }
        public void EditItem(TodoListItem item)
        {
            DomainEvents.Add(new ItemChanged { Item = item });
        }
    }
}
