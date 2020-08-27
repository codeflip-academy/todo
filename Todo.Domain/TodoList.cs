using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Core;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public partial class TodoList : Entity
    {
        public TodoList()
        {

        }

        public TodoList(string title)
        {
            ListTitle = title;

            DomainEvents.Add(new TodoListCreated { List = this });
        }

        public Guid Id { get; set; }
        public string ListTitle { get; set; }
        public bool Completed { get; private set; }
        public List<string> Contributors { get; private set; } = new List<string>();

        public TodoListItem CreateListItem(Guid id, string name, string notes, DateTime? dueDate, bool important)
        {
            var todoItem = new TodoListItem()
            {
                ListId = this.Id,
                Id = id,
                Name = name,
                Notes = notes,
                DueDate = dueDate,
                Important = important
            };

            DomainEvents.Add(new TodoListItemCreated { Item = todoItem, List = this });

            return todoItem;
        }
        public void SetCompleted(List<TodoListItem> items)
        {
            if (!items.All(item => item.ListId == Id) || items.Count == 0)
                return;

            var itemsCompleted = items.All(item => item.Completed);

            if (Completed && !itemsCompleted)
            {
                Completed = false;
                DomainEvents.Add(new TodoListCompletedStateChanged { List = this });
            }
            else if (!Completed && itemsCompleted)
            {
                Completed = true;
                DomainEvents.Add(new TodoListCompletedStateChanged { List = this });
            }
        }

        public void StoreContributor(string email, Guid senderAccountId)
        {
            DomainEvents.Add(new InvitationAccepted { List = this, Email = email, SenderAccountId = senderAccountId });
        }

        public bool DoesContributorExist(string inviteeEmail)
        {
            return this.Contributors.Exists(c => c == inviteeEmail);
        }

        public void AddContributor(string email)
        {
            if (email == null)
                return;
            Contributors.Add(email);
        }
        public void RemoveContribuor(string email)
        {
            if (email == null)
                return;
            Contributors.Remove(email);
        }
        public void UpdateListName()
        {
            DomainEvents.Add(new ListNameUpdated { List = this, });
        }

        public int GetContributorCountExcludingOwner()
        {
            return Contributors.Count - 1;
        }
    }
}
