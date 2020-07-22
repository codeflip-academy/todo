using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories.RoleChanges
{
    public class RoleChangeUserStory : AsyncRequestHandler<RoleChange>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPlanRepository _plan;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IAccountsListsRepository _accountsLists;
        private readonly ITodoListRepository _listRepository;

        public RoleChangeUserStory(IAccountRepository accountRepository,
         IPlanRepository plan,
         IAccountPlanRepository accountPlan,
         IAccountsListsRepository accountsLists,
         ITodoListRepository listRepository)
        {
            _accountRepository = accountRepository;
            _plan = plan;
            _accountPlan = accountPlan;
            _accountsLists = accountsLists;
            _listRepository = listRepository;
        }
        protected override async Task Handle(RoleChange request, CancellationToken cancellationToken)
        {
            var account =  await _accountRepository.FindAccountByIdAsync(request.AcountId);
            var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(request.AcountId);
            var accountsLists = await _accountsLists.FindAccountsListsByAccountIdAsync(request.AcountId);
            var listCount = accountPlan.ListCount;

            if (request.Plan == "Free")
            {
                if(accountPlan.IsNewPlanLessThanCurrentPlan(PlanTiers.Free) == true)
                {
                   foreach(var accountsList in accountsLists)
                    {
                        var list = await _listRepository.FindTodoListIdByIdAsync(accountsList.ListId);

                        if(listCount > PlanTiers.FreeMaxLists)
                        {
                            await _listRepository.RemoveTodoListAsync(list.Id);
                            listCount--;
                        }
                    }
                }
                accountPlan.PlanId = PlanTiers.Free;
            }
            else if (request.Plan == "Basic")
            {
                if(accountPlan.IsNewPlanLessThanCurrentPlan(PlanTiers.Basic) == true)
                {
                    foreach(var accountsList in accountsLists)
                    {
                        var list = await _listRepository.FindTodoListIdByIdAsync(accountsList.ListId);

                        if(listCount > PlanTiers.BasisMaxLists)
                        {
                            await _listRepository.RemoveTodoListAsync(list.Id);
                            listCount--;
                        }
                    }
                }
                accountPlan.PlanId = PlanTiers.Basic;
            }
            else if (request.Plan == "Premium")
            {
                accountPlan.PlanId = PlanTiers.Premium;
            }

            await _accountPlan.SaveChangesAsync();
        }
    }
}