using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories.EditItem
{
    public class EditItemUserStory : AsyncRequestHandler<EditItem>
    {
        private readonly ITodoListRepository _todoListRepository;
        private readonly ITodoListItemRepository _todoListItemRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IPlanRepository _planRepository;

        public EditItemUserStory(
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
        protected override async Task Handle(EditItem request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);
            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            var list = await _todoListRepository.FindTodoListIdByIdAsync(request.ListId);
            var item = await _todoListItemRepository.FindToDoListItemByIdAsync(request.ItemId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, request.Email);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var dueDate = accountPlanAuthorization.CanAddDueDate() ? request.DueDate : null;

                item.Name = request.Name;
                item.Notes = request.Notes;
                item.DueDate = dueDate;

                item.EditItem(item);

                await _todoListItemRepository.SaveChangesAsync();
            }
        }
    }
}
