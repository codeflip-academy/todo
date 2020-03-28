﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface ITodoListItemRepository : IRepository
    {
        Task AddTodoListItemAsync(TodoListItem todo);
        Task<TodoListItem> FindToDoListItemByIdAsync(int todoListItemId);
        Task RemoveTodoListItemAsync(int todo);
        Task RemoveAllTodoListItemsFromAccountAsync(int listId);
    }
}