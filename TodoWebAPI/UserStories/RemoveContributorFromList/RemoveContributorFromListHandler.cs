using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories
{
    public class RemoveContributorFromListHandler : AsyncRequestHandler<RemoveContributorFromList>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly ITodoListRepository _listRepository;
        private readonly IAccountsListsRepository _accountsListsRepository;

        public RemoveContributorFromListHandler(IAccountRepository accountRepository, 
        IAccountPlanRepository accountPlanRepository, 
        ITodoListRepository listRepository,
        IAccountsListsRepository accountsListsRepository)
        {
            _accountRepository = accountRepository;
            _accountPlanRepository = accountPlanRepository;
            _listRepository = listRepository;
            _accountsListsRepository = accountsListsRepository;
        }
        protected async override Task Handle(RemoveContributorFromList request, CancellationToken cancellationToken)
        {
            var contributor = await _accountRepository.FindAccountByEmailAsync(request.Email);
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(contributor.Id);
            var list = await _listRepository.FindTodoListIdByIdAsync(request.ListId);

            list.Contributors.Remove(contributor.Email);

            _listRepository.UpdateListAsync(list);

            var accountsLists = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(contributor.Id, request.ListId);
            accountsLists.MakeLeft();

            accountPlan.DecrementListCount();

            await _listRepository.SaveChangesAsync();
        }
    }
}