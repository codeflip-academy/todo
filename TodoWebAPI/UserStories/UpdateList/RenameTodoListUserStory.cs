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
    public class RenameTodoListUserStory : IRequestHandler<UpdateList, TodoList>
    {
        private readonly ITodoListRepository _repository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountsListsRepository _accountListsRepository;

        public RenameTodoListUserStory(ITodoListRepository repository, IAccountRepository accountRepository, IAccountsListsRepository accountListsRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _accountListsRepository = accountListsRepository;
        }
        public async Task<TodoList> Handle(UpdateList request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindAccountByEmailAsync(request.Email);
            var accountsLists = await _accountListsRepository.FindAccountsListsByAccountIdAndListIdAsync(account.Id, request.ListId);
            var todoList = await _repository.FindTodoListIdByIdAsync(request.ListId);

            if(accountsLists.UserIsOwner(account.Id))
            {
                todoList.ListTitle = request.ListTitle;

                todoList.UpdateListName();

                await _repository.SaveChangesAsync();

                return todoList;
            }

            return null;
        }
    }
}
