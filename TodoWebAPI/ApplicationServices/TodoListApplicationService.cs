﻿using MediatR;
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
        private readonly IMediator _mediator;

        public TodoListApplicationService(
            ITodoListRepository listRepository,
            IAccountRepository accountRepository,
            ITodoListItemRepository todoListItemRepository,
            IMediator mediator)
        {
            _listRepository = listRepository;
            _accountRepository = accountRepository;
            _todoListItemRepository = todoListItemRepository;
            _mediator = mediator;
        }

        public async Task<TodoList> CreateTodoListAsync(int accountId, string listTitle)
        {
            var doesAccountExist = await _accountRepository.DoesAccountWithAccountIdExistAsync(accountId);

            if (!doesAccountExist)
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

            await _listRepository.SaveChangesAsync();
        }

        public async Task DeleteTodoList(int listId)
        {
            await _todoListItemRepository.RemoveAllTodoListItemsFromAccountAsync(listId);
            await _listRepository.RemoveTodoListAsync(listId);
            await _listRepository.SaveChangesAsync();
        }

        public async Task MarkTodoListAsCompletedAsync(int listId)
        {
            var items = await _todoListItemRepository.FindAllTodoListItemsByListIdAsync(listId);
            var list = await _listRepository.FindTodoListIdByIdAsync(listId);

            list.SetCompleted(items);
            await _listRepository.SaveChangesAsync();

            foreach (var domainEvent in list.DomainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}