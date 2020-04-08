using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;

namespace Todo.WebAPI.ApplicationServices
{
    public class TodoListItemApplicationService
    {
        private readonly ITodoListRepository _listRepository;
        private readonly ITodoListItemRepository _listItemRepository;

        public TodoListItemApplicationService(ITodoListRepository listRepository, ITodoListItemRepository todoListItemRepository)
        {
            _listRepository = listRepository;
            _listItemRepository = todoListItemRepository;
        }

        public async Task<TodoListItem> CreateTodoListItemAsync(int listId, int? parentId, int accountId, string todoName, string notes)
        {
            var list = await _listRepository.FindTodoListIdByIdAsync(listId);

            if (list == null)
                return null;

            var todoItem = new TodoListItem()
            {
                ListId = listId,
                ParentId = parentId,
                ToDoName = todoName,
                Notes = notes,
                AccountId = accountId,
                Position = list.GetNextPosition()
            };

            await _listItemRepository.AddTodoListItemAsync(todoItem);
            return todoItem;
        }

        public async Task UpdateTodoListItemAsync(int todoListItemId, string notes, string todoName)
        {
            var todoListItem = await _listItemRepository.FindToDoListItemByIdAsync(todoListItemId);

            todoListItem.Notes = notes;
            todoListItem.ToDoName = todoName;
        }

        public async Task ChangeTodoListItemsPositionAsync(int todoListItem1Id, int todoListItem2Id)
        {
            var item1 = await _listItemRepository.FindToDoListItemByIdAsync(todoListItem1Id);
            var item2 = await _listItemRepository.FindToDoListItemByIdAsync(todoListItem2Id);

            var item1Position = item1.Position;
            var item2Position = item2.Position;

            item1.Position = item2Position;
            item2.Position = item1Position;
        }

        public async Task DeleteTodoListItem(int todoListItemId)
        {
            await _listItemRepository.RemoveTodoListItemAsync(todoListItemId);
        }

        public async Task MarkTodoListItemAsCompletedAsync(int todoListItemId, bool state)
        {
            var item = await _listItemRepository.FindToDoListItemByIdAsync(todoListItemId);

            if(state == true)
            {
                item.SetCompleted();
            }
            else if(state == false)
            {
                item.SetNotCompleted();
            }

            await _listItemRepository.SaveChangesAsync();
        }
    }
}
