using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.InfrastructureEventHandlers
{
    public class StoreContributorInList : INotificationHandler<InvitationAccepted>
    {
        private readonly ITodoListRepository _todoList;

        public StoreContributorInList(ITodoListRepository todoList)
        {
            _todoList = todoList;
        }
        public async Task Handle(InvitationAccepted notification, CancellationToken cancellationToken)
        {
            notification.List.AddContributor(notification.Email);

            _todoList.UpdateListAsync(notification.List);

            await _todoList.SaveChangesAsync();
        }
    }
}
