using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TodoWebAPI.Models;
using System.Data.SqlClient;
using Dapper;
using Todo.WebAPI.ApplicationServices;
using Todo.Domain.Repositories;
using Todo.Infrastructure;

namespace TodoWebAPI.Controllers
{
    public class TodoListItemController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly TodoListItemApplicationService _todoListItemApplicationService;
        private readonly TodoDatabaseContext _todoDatabaseContext;

        public TodoListItemController(IConfiguration config, TodoListItemApplicationService todoListItemApplicationService, TodoDatabaseContext todoDatabaseContext)
        {
            _config = config;
            _todoListItemApplicationService = todoListItemApplicationService;
            _todoDatabaseContext = todoDatabaseContext;
        }


        [HttpPost("accounts/{accountId}/lists/{listId}/todos")]
        public async Task<IActionResult> CreateTodo(int accountId, int listId, [FromBody] CreateToDoModel todo)
        {
            var todoItem = await _todoListItemApplicationService.CreateTodoListItemAsync(listId, todo.ParentId, accountId, todo.ToDoName, todo.Notes);
            
            if (todoItem == null)
              return BadRequest("List doesn't exist");

             await _todoDatabaseContext.SaveChangesAsync();

            return Ok(new TodoListItemModel()
            {
                Id = todoItem.Id,
                ToDoName = todoItem.ToDoName,
                ParentId = todoItem.ParentId,
                Notes = todoItem.Notes,
                ListId = listId
            });
        }

        [HttpGet("accounts/{accountId}/lists/{listId}/todos")]
        public async Task<IActionResult> GetAllTodoItemsAsync(int accountId, int listId)
        {
            using (var connection = new SqlConnection(_config.GetSection("ConnectionStrings")["Development"]))
            {
                await connection.OpenAsync();

                var todoList = await connection.QueryAsync<TodoListItemModel>("SELECT * From TodoListItems Where AccountID = @accountId AND ListID = @listId", new { accountId = accountId, listId = listId });

                return Ok(todoList);
            }
        }

        [HttpPut("accounts/{accountId}/todos/{todoId}")]
        public async Task<IActionResult> EditTodoAsync(int accountId, int todoId, [FromBody] TodoListItemModel todo)
        {
            await _todoListItemApplicationService.UpdateTodoListItemAsync(todoId, todo.Notes, todo.ToDoName);

            await _todoDatabaseContext.SaveChangesAsync();
            
            return Ok($"Name = {todo.ToDoName}, Notes = {todo.Notes}");
        }

        [HttpPut("accounts/{accountId}/lists/{listId}/todos/position")]
        public async Task<IActionResult> SwapPositions([FromBody] TodoListItemPostionModel todoListItemPostionModel)
        {
            await _todoListItemApplicationService.ChangeTodoListItemsPositionAsync(todoListItemPostionModel.TodoListItemId1, todoListItemPostionModel.TodoListItemId2);

            await _todoDatabaseContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("accounts/{accountId}/todos/{todoId}/completed")]
        public async Task<IActionResult> ToggleCompletedState(int accountId, int todoId, [FromBody] bool completed)
        {
            await _todoListItemApplicationService.MarkTodoListItemAsCompletedAsync(todoId, completed);


            return Ok();
        }


        [HttpDelete("accounts/{accountId}/todos/{todoId}")]
        public async Task<IActionResult> DeleteTodo(int accountId, int todoId)
        {
            await _todoListItemApplicationService.DeleteTodoListItem(todoId);

            await _todoDatabaseContext.SaveChangesAsync();

            return Ok("Todo list item deleted.");
        }
    }
}