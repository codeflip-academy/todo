using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface ITodoListRepository : IRepository
    {
        Task AddTodoListAsync(TodoList list, Guid accountId);
        Task<List<TodoList>> FindTodoListsByAccountIdAsync(Guid accountId);
        Task<TodoList> FindTodoListIdByIdAsync(Guid listId);
        Task<List<TodoList>> GetNumberOfTodoListsByAccountIdAsync(Guid accountId, int numberOfLists);
        Task RemoveTodoListAsync(Guid listId);
        void UpdateListAsync(TodoList list);
        Task<List<TodoList>> GetOwnedListsAsync(Guid accountId);
        Task<List<TodoList>> GetUnOwnedListsAsync(Guid accountId);
    }
}