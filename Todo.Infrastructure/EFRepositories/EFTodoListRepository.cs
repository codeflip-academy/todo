using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Core;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.Data
{
    public class EFTodoListRepository : ITodoListRepository
    {
        private readonly ISequentialIdGenerator _idGenerator;

        private TodoDatabaseContext _context { get; set; }
        public EFTodoListRepository(TodoDatabaseContext context, ISequentialIdGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public Task AddTodoListAsync(TodoList todoList, Guid accountId)
        {
            _context.TodoLists.Add(todoList);
            var accountLists = new RoleOwner()
            {
                Id = _idGenerator.NextId(),
                AccountId = accountId,
                ListId = todoList.Id,
            };

            accountLists.Owned();

            _context.AccountsLists.Add(accountLists);
            return Task.CompletedTask;
        }

        public async Task<List<TodoList>> FindTodoListsByAccountIdAsync(Guid accountId)
        {
            var todoLists =
                (from list in _context.TodoLists
                 join accountList in _context.AccountsLists on list.Id equals accountList.ListId
                 where (accountList.Role == Roles.Contributor || accountList.Role == Roles.Owner) && accountList.AccountId == accountId
                 select list).ToList();

            return todoLists;
        }

        public async Task<TodoList> FindTodoListIdByIdAsync(Guid listId)
        {
            return await _context.TodoLists.FindAsync(listId);
        }
        public async Task RemoveTodoListAsync(Guid listId)
        {
            var list = await _context.TodoLists.FindAsync(listId);

            _context.Remove(list);
        }
        public async Task<List<TodoList>> GetNumberOfTodoListsByAccountIdAsync(Guid accountId, int numberOfLists)
        {
            var todoLists = (from list in _context.TodoLists
                             join accountList in _context.AccountsLists on list.Id equals accountList.ListId
                             where accountList.AccountId == accountId && accountList.Role != Roles.Invited
                             && accountList.Role != Roles.Left
                             && accountList.Role != Roles.Declined
                             select list).Take(numberOfLists).ToList();

            return todoLists;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public Guid NextId()
        {
            return _idGenerator.NextId();
        }

        public void UpdateListAsync(TodoList list)
        {
            _context.TodoLists.Update(list);
        }

        public async Task<List<TodoList>> GetOwnedListsAsync(Guid accountId)
        {
            var todoLists = (from list in _context.TodoLists
                             join accountList in _context.AccountsLists on list.Id equals accountList.ListId
                             where accountList.Role == Roles.Owner && accountList.AccountId == accountId
                             select list).ToList();

            return todoLists;
        }

        public async Task<List<TodoList>> GetUnOwnedListsAsync(Guid accountId)
        {
            var todoLists = (from list in _context.TodoLists
                             join accountList in _context.AccountsLists on list.Id equals accountList.ListId
                             where accountList.Role == Roles.Contributor && accountList.AccountId == accountId
                             select list).ToList();

            return todoLists;
        }
    }
}