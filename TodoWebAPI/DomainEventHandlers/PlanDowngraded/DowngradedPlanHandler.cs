using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace TodoWebAPI.DomainEventHandlers
{
    public class DowngradedPlanHandler : INotificationHandler<PlanDowngraded>
    {
        private readonly IDowngradeRepository _downgradeRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ITodoListRepository _listRepository;
        private readonly IAccountsListsRepository _accountsListsRepository;
        private readonly IAccountRepository _accountsRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;

        public DowngradedPlanHandler(IDowngradeRepository downgradeRepository,
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
        public async Task Handle(PlanDowngraded notification, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(notification.Downgrade.AccountId);
            var plan = await _planRepository.FindPlanByIdAsync(notification.Downgrade.PlanId);
            var account = await _accountsRepository.FindAccountByIdAsync(notification.Downgrade.AccountId);

            var numListsToDelete = (accountPlan.ListCount - plan.MaxLists) < 0 ? 0 : accountPlan.ListCount - plan.MaxLists;
            var listsToDelete = await _listRepository.GetNumberOfTodoListsByAccountIdAsync(notification.Downgrade.AccountId, numListsToDelete);

            foreach (var list in listsToDelete)
            {
                var accountListOwner = await _accountsListsRepository.FindAccountsListsOwnerByAccountIdAndListIdAsync(notification.Downgrade.AccountId, list.Id);
                var accountListContributor = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAndListIdAsync(notification.Downgrade.AccountId, list.Id);

                if (accountListOwner != null)
                {
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
                            list.Contributors.Remove(contributorAccount.Email);
                            _listRepository.UpdateListAsync(list);
                        }
                    }

                    await _listRepository.RemoveTodoListAsync(list.Id);
                }
                else if (accountListContributor != null)
                {
                    accountListContributor.MakeLeft();
                    accountPlan.DecrementListCount();
                    list.Contributors.Remove(account.Email);
                    _listRepository.UpdateListAsync(list);

                }
                await _listRepository.SaveChangesAsync();
            }
        }
    }
}