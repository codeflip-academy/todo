using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface ITodoListRepository : IRepository
    {
        Task AddTodoListAsync(TodoList list, Guid accountId);
        Task<List<TodoList>> FindTodoListsByAccountIdAsync(Guid accountId, int pageSize);
        Task<TodoList> FindTodoListIdByIdAsync(Guid listId);
        Task RemoveTodoListAsync(Guid listId);
        void UpdateListAsync(TodoList list);
    }
}