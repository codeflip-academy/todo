using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Guids;

namespace TodoWebAPI.UserStories.CreateItem
{
    public class CreateItemUserStory : IRequestHandler<CreateItem, TodoListItem>
    {
        private readonly ITodoListRepository _todoListRepository;
        private readonly ITodoListItemRepository _todoListItemRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IPlanRepository _planRepository;

        public CreateItemUserStory(
            ITodoListRepository todoListRepository,
            ITodoListItemRepository todoListItemRepository,
            IAccountPlanRepository accountPlanRepository,
            IPlanRepository planRepository)
        {
            _todoListRepository = todoListRepository;
            _todoListItemRepository = todoListItemRepository;
            _accountPlanRepository = accountPlanRepository;
            _planRepository = planRepository;
        }
        public async Task<TodoListItem> Handle(CreateItem request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);

            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            var list = await _todoListRepository.FindTodoListIdByIdAsync(request.ListId);

            var todoListAuthorization = new TodoListAuthorizationValidator(list.Contributors, request.Email);

            if(todoListAuthorization.IsUserAuthorized())
            {
                if (list == null)
                    return null;

                var dueDate = accountPlanAuthorization.CanAddDueDate() ? request.DueDate : null;

                var id = _todoListItemRepository.NextId();

                var todoItem = list.CreateListItem(id, request.Name, request.Notes, dueDate);

                todoItem.AccountId = request.AccountId;

                await _todoListItemRepository.AddTodoListItemAsync(todoItem);

                await _todoListItemRepository.SaveChangesAsync();

                return todoItem;
            }

            return null;
        }
    }
}
