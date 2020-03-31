﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Repositories;

namespace Todo.Domain.Services
{
    public class TodoListItemService
    {
        private readonly ITodoListRepository _listRepository;
        private readonly ITodoListItemRepository _listItemRepository;
        private readonly IMediator _mediator;

        public TodoListItemService(ITodoListRepository listRepository, ITodoListItemRepository todoListItemRepository, IMediator mediator)
        {
            _listRepository = listRepository;
            _listItemRepository = todoListItemRepository;
            _mediator = mediator;
        }

        public async Task<bool> CreateTodoListItemAsync(int listId, int? parentId, int accountId, bool completed, string todoName, string notes)
        {
            var doesListExist = await _listRepository.FindTodoListIdByIdAsync(listId);

            if (doesListExist == null)
                return false;

            var todoItem = new TodoListItem()
            {
                ListId = listId,
                ParentId = parentId,
                Completed = completed,
                ToDoName = todoName,
                Notes = notes,
                AccountId = accountId
            };

            await _listItemRepository.AddTodoListItemAsync(todoItem);
            return true;
        }

        public async Task UpdateTodoListItemAsync(int todoListItemId, string notes, string todoName, bool completed)
        {
            var todoListItem = await _listItemRepository.FindToDoListItemByIdAsync(todoListItemId);

            todoListItem.Notes = notes;
            todoListItem.ToDoName = todoName;
            todoListItem.Completed = completed;

            await _listItemRepository.SaveChangesAsync();

            await _mediator.Publish(new TodoListItemUpdated(){Item = todoListItem});
        }

        public async Task DeleteTodoListItem(int todoListItemId)
        {
            await _listItemRepository.RemoveTodoListItemAsync(todoListItemId);
            await _listItemRepository.SaveChangesAsync();
        }
    }
}
