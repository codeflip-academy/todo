using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories
{
    public class DeclineInvitationHandler : AsyncRequestHandler<DeclineInvitation>
    {
        private readonly IAccountsListsRepository _accountsListsRepository;

        public DeclineInvitationHandler(IAccountsListsRepository accountsListsRepository)
        {
            _accountsListsRepository = accountsListsRepository;
        }

        protected override async Task Handle(DeclineInvitation request, CancellationToken cancellationToken)
        {
            var accountsLists = await _accountsListsRepository.FindAccountsListsInvitedByAccountIdAsync(request.AccountId, request.ListId);
            accountsLists.MakeDeclined();

            await _accountsListsRepository.SaveChangesAsync();
        }
    }
}