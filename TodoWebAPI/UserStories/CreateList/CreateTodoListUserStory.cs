using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories
{
    public class CreateTodoListUserStory : IRequestHandler<CreateList, TodoList>
    {
        private readonly ITodoListRepository _repository;
        private readonly IAccountPlanRepository _accountPlanRepository;

        public CreateTodoListUserStory(ITodoListRepository repository, IAccountPlanRepository accountPlanRepository)
        {
            _repository = repository;
            _accountPlanRepository = accountPlanRepository;
        }
        public async Task<TodoList> Handle(CreateList request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.ListTitle))
                return null;

            var todoList = new TodoList (request.ListTitle);

            todoList.AccountId = request.AccountId;

            todoList.Contributors.Add(request.Email);

            todoList.Id = _repository.NextId();

            await _repository.AddTodoListAsync(todoList, request.AccountId);

            await _repository.SaveChangesAsync();

            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.AccountId);

            accountPlan.IncrementListCount();

            await _accountPlanRepository.SaveChangesAsync();

            return todoList;
        }
    }
}