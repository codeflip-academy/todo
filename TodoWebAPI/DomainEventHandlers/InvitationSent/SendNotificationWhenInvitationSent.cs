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
using Todo.Infrastructure.Dto;
using TodoWebAPI.Models;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers.Invitation
{
    public class SendInvitationToClientWhenInvitaionSent : INotificationHandler<InvitationSent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountsListsRepository _accountsListsRepository;

        public SendInvitationToClientWhenInvitaionSent(
            IHubContext<NotificationHub> hubContext,
            ITodoListRepository todoListRepository,
            IAccountRepository accountRepository,
            IAccountsListsRepository accountsListsRepository)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
            _accountRepository = accountRepository;
            _accountsListsRepository = accountsListsRepository;
        }

        public async Task Handle(InvitationSent notification, CancellationToken cancellationToken)
        {
            var list = await _todoListRepository.FindTodoListIdByIdAsync(notification.ListId);
            var invitee = await _accountRepository.FindAccountByIdAsync(notification.InviteeAccountId);
            var accountsLists = await _accountsListsRepository.FindAccountsListsByAccountIdAndListIdAsync(invitee.Id, list.Id);

            var listModel = new TodoListDto()
            {
                Id = list.Id,
                Contributors = list.Contributors,
                Role = accountsLists.Role,
                ListTitle = list.ListTitle,
                Completed = list.Completed
            };

            await _hubContext.Clients.User(invitee.Email).SendAsync("InvitationSent", listModel);
        }
    }
}
