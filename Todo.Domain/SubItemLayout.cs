using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Todo.Domain.DomainEvents;

namespace Todo.Domain
{
    public class SubItemLayout : Entity
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public List<Guid> Layout { get; set; } = new List<Guid>();
        public void UpdateLayout(Guid subItemId, int position, Guid itemId)
        {
            Layout.Remove(subItemId);
            Layout.Insert(position, subItemId);

            if (Layout.Distinct().Count() != Layout.Count)
            {
                throw new ArgumentException("Layout contains duplicate Id.");
            }
            else
            {
                DomainEvents.Add(new ItemLayoutUpdated { Position = position, SubItemId = subItemId, ItemId = itemId });
            }
        }

        public void RemoveSubItemFromLayout(Guid subItemId)
        {
            Layout.Remove(subItemId);
        }
    }
}
