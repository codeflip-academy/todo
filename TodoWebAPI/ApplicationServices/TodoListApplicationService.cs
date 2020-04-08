using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace Todo.WebAPI.ApplicationServices
{
    public class TodoListApplicationService
    {
        private readonly ITodoListRepository _listRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITodoListItemRepository _todoListItemRepository;

        public TodoListApplicationService(
            ITodoListRepository listRepository,
            IAccountRepository accountRepository,
            ITodoListItemRepository todoListItemRepository)
        {
            _listRepository = listRepository;
            _accountRepository = accountRepository;
            _todoListItemRepository = todoListItemRepository;
        }

        public async Task<TodoList> CreateTodoListAsync(int accountId, string listTitle)
        {
            if (String.IsNullOrEmpty(listTitle))
                return null;

            var todoList = new TodoList()
            {
                AccountId = accountId,
                ListTitle = listTitle
            };
            await _listRepository.AddTodoListAsync(todoList);

            return todoList;
        }

        public async Task RenameTodoListAsync(int listId, string listTitle)
        {
            var todoList = await _listRepository.FindTodoListIdByIdAsync(listId);

            todoList.ListTitle = listTitle;
        }

        public async Task DeleteTodoList(int listId)
        {
            await _todoListItemRepository.RemoveAllTodoListItemsFromAccountAsync(listId);
            await _listRepository.RemoveTodoListAsync(listId);
        }

        public async Task MarkTodoListAsCompletedAsync(int listId)
        {
            var items = await _todoListItemRepository.FindAllTodoListItemsByListIdAsync(listId);
            var list = await _listRepository.FindTodoListIdByIdAsync(listId);

            list.SetCompleted(items);
            await _todoListItemRepository.SaveChangesAsync();
        }
    }
}