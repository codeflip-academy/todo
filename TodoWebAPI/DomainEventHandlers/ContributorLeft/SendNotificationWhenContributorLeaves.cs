using MediatR;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepository _accountRepository;

        public SendNotificationWhenContributorLeaves(
            IHubContext<NotificationHub> hubContext,
            ITodoListRepository todoListRepository,
            IAccountRepository accountsLists,
            IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
        }
        public async Task Handle(ContributorLeft notification, CancellationToken cancellationToken)
        {
            var list = await _todoListRepository.FindTodoListIdByIdAsync(notification.ListId.GetValueOrDefault());
            var account = await _accountRepository.FindAccountByIdAsync(notification.AccountId);

            var usersExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, account.Email);

            await _hubContext.Clients.Users(usersExceptYou).SendAsync("ContributorLeft", list);
        }

    }
}
