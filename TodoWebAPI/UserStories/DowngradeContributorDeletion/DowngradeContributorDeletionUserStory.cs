using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories
{
    public class DowngradeContributorDeletionUserStory : AsyncRequestHandler<DowngradeContributorDeletion>
    {
        private readonly IAccountsListsRepository _accountsListsRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IAccountRepository _accountRepository;

        public DowngradeContributorDeletionUserStory(
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

        protected override async Task Handle(DowngradeContributorDeletion request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);
            var lists = await _todoListRepository.FindTodoListsByAccountIdAsync(request.AccountId);
            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            foreach (var list in lists)
            {
                var accountList = await _accountsListsRepository.FindAccountsListsByAccountIdAndListIdAsync(request.AccountId, list.Id);

                if (plan.MaxContributors == 0)
                {
                    var account = await _accountRepository.FindAccountByIdAsync(request.AccountId);

                    var contributorAccountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(request.AccountId, list.Id);
                    var contributorAccountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);

                    contributorAccountList.MakeLeft();
                    contributorAccountPlan.DecrementListCount();
                    list.Contributors.Remove(account.Email);
                }

                else
                {
                    var numContributorsToRemove = list.GetContributorCountExcludingOwner() - plan.MaxContributors;
                    var account = await _accountRepository.FindAccountByIdAsync(request.AccountId);

                    for (var i = 0; i < numContributorsToRemove; i++)
                    {
                        var contributors = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, account.Email);
                        foreach (var contributor in contributors)
                        {
                            var accountContributor = await _accountRepository.FindAccountByEmailAsync(contributor);
                            var contributorAccountList = await _accountsListsRepository.FindAccountsListsContributorByAccountIdAsync(request.AccountId, list.Id);
                            var contributorAccountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);

                            contributorAccountList.MakeLeft();
                            contributorAccountPlan.DecrementListCount();
                            list.Contributors.Remove(accountContributor.Email);

                            await _accountsListsRepository.SaveChangesAsync();
                        }

                    }
                }
            }
        }
    }
}