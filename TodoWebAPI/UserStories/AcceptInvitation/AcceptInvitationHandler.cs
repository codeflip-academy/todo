using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories
{
    public class AcceptInvitationHandler : AsyncRequestHandler<AcceptInvitaion>
    {
        private readonly ITodoListRepository _listRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IAccountsListsRepository _accountsListsRepository;
        private readonly IPlanRepository _planRepository;

        public AcceptInvitationHandler(
            ITodoListRepository listRepository,
            IAccountRepository accountRepository,
            IAccountPlanRepository accountPlan,
            IAccountsListsRepository accountsListsRepository,
            IPlanRepository planRepository)
        {
            _listRepository = listRepository;
            _accountRepository = accountRepository;
            _accountPlan = accountPlan;
            _accountsListsRepository = accountsListsRepository;
            _planRepository = planRepository;
        }

        protected override async Task Handle(AcceptInvitaion request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(request.AccountId);
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);
            var account = await _accountRepository.FindAccountByIdAsync(request.AccountId);
            var list = await _listRepository.FindTodoListIdByIdAsync(request.ListId);

            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            if (accountPlanAuthorization.CanCreateList())
            {
                var accountsLists = await _accountsListsRepository.FindAccountsListsInvitedByAccountIdAsync(request.AccountId, request.ListId);
                accountsLists.MakeContributor();

                accountPlan.IncrementListCount();
                list.StoreContributor(account.Email, request.AccountId);
                await _listRepository.SaveChangesAsync();
            }
        }
    }
}