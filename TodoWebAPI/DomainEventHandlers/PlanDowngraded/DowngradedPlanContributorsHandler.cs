using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.DomainEventHandlers
{
    public class DowngradedPlanContributorsHandler : INotificationHandler<PlanDowngraded>
    {
        private readonly IAccountsListsRepository _accountsListsRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IAccountRepository _accountRepository;

        public DowngradedPlanContributorsHandler(
            IAccountsListsRepository accountsListsRepository,
            IAccountPlanRepository accountPlanRepository,
            IPlanRepository planRepository,
            ITodoListRepository todoListRepository,
            IAccountRepository accountRepository)
        {
            _accountsListsRepository = accountsListsRepository;
            _accountPlanRepository = accountPlanRepository;
            _planRepository = planRepository;
            _todoListRepository = todoListRepository;
            _accountRepository = accountRepository;
        }
        public async Task Handle(PlanDowngraded notification, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(notification.Downgrade.AccountId);
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);
            var myAccount = await _accountRepository.FindAccountByIdAsync(notification.Downgrade.AccountId);
            var unownedLists = await _todoListRepository.GetUnOwnedListsAsync(notification.Downgrade.AccountId);
            var ownedLists = await _todoListRepository.GetOwnedListsAsync(notification.Downgrade.AccountId);

            if (plan.MaxContributors == 0)
            {
                foreach (var ownedList in ownedLists)
                {
                    var contributors = RemoveSelfFromContributorSignalRHelper.RemoveContributor(ownedList, myAccount.Email);
                    var contributorsCount = contributors.Count;

                    for (var i = 0; i < contributorsCount; i++)
                    {
                        var contributor = contributors[i];

                        var account = await _accountRepository.FindAccountByEmailAsync(contributor);
                        var contributorAccountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(account.Id, ownedList.Id);
                        var contributorAccountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(account.Id);

                        contributorAccountList.MakeLeft();
                        contributorAccountPlan.DecrementListCount();
                        ownedList.Contributors.Remove(contributor);

                        await _todoListRepository.SaveChangesAsync();
                    }
                }

                foreach (var unownedList in unownedLists)
                {
                    var accountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(notification.Downgrade.AccountId, unownedList.Id);

                    accountList.MakeLeft();
                    accountPlan.DecrementListCount();
                    unownedList.Contributors.Remove(myAccount.Email);

                    await _todoListRepository.SaveChangesAsync();
                }
            }
            else
            {
                var contributorCount = ownedLists.Count;

                if (plan.MaxContributors < contributorCount)
                {
                    foreach (var ownedList in ownedLists)
                    {
                        var contributors = RemoveSelfFromContributorSignalRHelper.RemoveContributor(ownedList, myAccount.Email);

                        foreach (var contributor in contributors)
                        {
                            var accountContributor = await _accountRepository.FindAccountByEmailAsync(contributor);
                            var contributorAccountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(notification.Downgrade.AccountId, ownedList.Id);
                            var contributorAccountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(notification.Downgrade.AccountId);

                            contributorAccountList.MakeLeft();
                            contributorAccountPlan.DecrementListCount();
                            ownedList.Contributors.Remove(accountContributor.Email);

                            await _accountsListsRepository.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}