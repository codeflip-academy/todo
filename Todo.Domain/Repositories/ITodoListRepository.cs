using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface ITodoListRepository : IUnitOfWork
    {
        Task AddTodoListAsync(TodoList list);
        Task<List<TodoList>> FindTodoListsByAccountIdAsync(int accountId, int pageSize);
        Task<TodoList> FindTodoListIdByIdAsync(int listId);
        Task RemoveTodoListAsync(int listId);
        Task RemoveAllTodoListsFromAccountAsync(int accountId);
    }
}