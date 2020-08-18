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
            var account = await _accountsRepository.FindAccountByIdAsync(notification.Downgrade.AccountId);

            foreach (var list in notification.NumberOfListsToDelete)
            {
                var accountList = await _accountsListsRepository.FindAccountsListsByAccountIdAndListIdAsync(notification.Downgrade.AccountId, list.Id);

                if (accountList.UserIsOwner(notification.Downgrade.AccountId))
                {
                    await _listRepository.RemoveTodoListAsync(list.Id);

                    var contributors = list.Contributors;

                    foreach (var contributor in contributors)
                    {
                        var contributorAccount = await _accountsRepository.FindAccountByEmailAsync(contributor);
                        var contributorPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(contributorAccount.Id);
                        var accountsListsContributor = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(contributorAccount.Id, list.Id);

                        if (accountsListsContributor != null)
                        {
                            accountsListsContributor.MakeLeft();
                            contributorPlan.DecrementListCount();
                        }
                    }

                    await _listRepository.SaveChangesAsync();
                }
                else
                {
                    var contributorAccountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(notification.Downgrade.AccountId, list.Id);
                    var contributorAccountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(notification.Downgrade.AccountId);

                    contributorAccountList.MakeLeft();
                    contributorAccountPlan.DecrementListCount();
                    list.Contributors.Remove(account.Email);

                    await _accountsListsRepository.SaveChangesAsync();
                }
            }
        }
    }
}