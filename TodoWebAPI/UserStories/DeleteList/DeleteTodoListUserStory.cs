using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories
{
    public class DeleteTodoListUserStory : AsyncRequestHandler<DeleteList>
    {
        private readonly ITodoListItemRepository _todoListItem;
        private readonly ITodoListRepository _listRepository;
        private readonly IAccountPlanRepository _accountPlan;
        private readonly IAccountRepository _accountRepository;

        public DeleteTodoListUserStory(ITodoListItemRepository todoListItem, ITodoListRepository listRepository, IAccountPlanRepository accountPlan, IAccountRepository accountRepository)
        {
            _todoListItem = todoListItem;
            _listRepository = listRepository;
            _accountPlan = accountPlan;
            _accountRepository = accountRepository;
        }
        protected override async Task Handle(DeleteList request, CancellationToken cancellationToken)
        {
            var list = await _listRepository.FindTodoListIdByIdAsync(request.ListId);

            await _listRepository.RemoveTodoListAsync(request.ListId);

            var accountPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(request.AccountId);
            
            var contributors = list.Contributors;

            foreach(var contributor in contributors)
            {
                var account = await _accountRepository.FindAccountByEmailAsync(contributor);
                var contributorPlan = await _accountPlan.FindAccountPlanByAccountIdAsync(account.Id);
                contributorPlan.DecrementListCount();
            }

            await _listRepository.SaveChangesAsync();
        }
    }
}
