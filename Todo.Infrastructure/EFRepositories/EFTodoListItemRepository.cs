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
    public class EFTodoListItemRepository : ITodoListItemRepository
    {
        private readonly TodoDatabaseContext _context;
        private readonly ISequentialIdGenerator _idGenerator;

        public EFTodoListItemRepository(TodoDatabaseContext context, ISequentialIdGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public Task AddTodoListItemAsync(TodoListItem todo)
        {
            _context.TodoListItems.Add(todo);
            return Task.CompletedTask;
        }
        public void RemoveTodoListItemAsync(TodoListItem todo)
        {
            _context.Remove(todo);
        }
        public async Task<TodoListItem> FindToDoListItemByIdAsync(Guid todoListItemId)
        {
            return await _context.TodoListItems.FindAsync(todoListItemId);
        }
        public async Task<List<TodoListItem>> FindAllTodoListItemsByListIdAsync(Guid listId)
        {
            return await _context.TodoListItems.Where(x => x.ListId == listId).ToListAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetSubItemCountAsync(Guid listItemId)
        {
            return await _context.SubItems.CountAsync(x => x.ListItemId == listItemId);
        }

        public Guid NextId()
        {
            return _idGenerator.NextId();
        }
    }
}