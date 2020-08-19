using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories
{
    public class DowngradeDeleteListsStory : IRequestHandler<DowngradeDeleteLists, bool>
    {
        private readonly IDowngradeRepository _downgradeRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ITodoListRepository _listRepository;
        private readonly IAccountsListsRepository _accountsListsRepository;
        private readonly IAccountRepository _accountsRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;

        public DowngradeDeleteListsStory(IDowngradeRepository downgradeRepository,
         IAccountPlanRepository accountPlanRepository,
         IPlanRepository planRepository,
         ITodoListRepository listRepository,
         IAccountsListsRepository accountsListsRepository,
         IAccountRepository accountsRepository)
        {
            _downgradeRepository = downgradeRepository;
            _accountPlanRepository = accountPlanRepository;
            _planRepository = planRepository;
            _listRepository = listRepository;
            _accountsListsRepository = accountsListsRepository;
            _accountsRepository = accountsRepository;
        }

        public async Task<bool> Handle(DowngradeDeleteLists request, CancellationToken cancellationToken)
        {
            var account = await _accountsRepository.FindAccountByIdAsync(request.AccountId);

            foreach (var list in request.NummberOfListsToDelete)
            {
                var accountList = await _accountsListsRepository.FindAccountsListsByAccountIdAndListIdAsync(request.AccountId, list.Id);

                if (accountList.UserIsOwner(request.AccountId))
                {
                    await _listRepository.RemoveTodoListAsync(list.Id);

                    var contributors = list.Contributors;

                    foreach (var contributor in contributors)
                    {
                        var contributorAccount = await _accountsRepository.FindAccountByEmailAsync(contributor);
                        var contributorPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(contributorAccount.Id);
                        var accountsListsContributor = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAndListIdAsync(contributorAccount.Id, list.Id);

                        if (accountsListsContributor != null)
                        {
                            accountsListsContributor.MakeLeft();
                            contributorPlan.DecrementListCount();
                        }
                    }

                    await _listRepository.SaveChangesAsync();

                    return true;
                }

                // Remove user from list and decrement list count
                else
                {
                    var contributorAccountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAndListIdAsync(request.AccountId, list.Id);
                    var contributorAccountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);

                    contributorAccountList.MakeLeft();
                    contributorAccountPlan.DecrementListCount();
                    list.Contributors.Remove(account.Email);

                    await _accountsListsRepository.SaveChangesAsync();
                }

            }
            return false;
        }
    }
}