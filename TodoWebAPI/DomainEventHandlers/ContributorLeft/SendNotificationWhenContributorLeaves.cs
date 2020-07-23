using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationWhenContributorLeaves : INotificationHandler<ContributorLeft>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IAccountRepository _accounts;

        public SendNotificationWhenContributorLeaves(
            IHubContext<NotificationHub> hubContext,
            ITodoListRepository todoListRepository,
            IAccountRepository accountsLists)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
            _accounts = accountsLists;
        }
        public async Task Handle(ContributorLeft notification, CancellationToken cancellationToken)
        {
            var list = await _todoListRepository.FindTodoListIdByIdAsync(notification.ListId);

            var account = await _accounts.FindAccountByIdAsync(notification.AccountId);

            var usersExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, account);

            await _hubContext.Clients.Users(usersExceptYou).SendAsync("ContributorLeft", list);
        }

        
    }
}
