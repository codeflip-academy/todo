using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TodoWebAPI.Models;
using TodoWebAPI.UserStories.CreateItem;
using MediatR;
using TodoWebAPI.UserStories.EditItem;
using TodoWebAPI.UserStories.ItemLayout;
using TodoWebAPI.UserStories.TrashItem;
using TodoWebAPI.UserStories.ItemCompletedState;
using TodoWebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Todo.Domain;
using Todo.Domain.Repositories;
using TodoWebAPI.ViewModels;
using TodoWebAPI.UserStories;

namespace TodoWebAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class TodoListItemController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMediator _mediator;
        private readonly DapperQuery _dapperQuery;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IPlanRepository _planRepository;

        public TodoListItemController(
            IConfiguration config,
            IMediator mediator,
            DapperQuery dapperQuery,
            IAccountPlanRepository accountPlanRepository,
            IPlanRepository planRepository)
        {
            _config = config;
            _mediator = mediator;
            _dapperQuery = dapperQuery;
            _accountPlanRepository = accountPlanRepository;
            _planRepository = planRepository;
        }


        [HttpPost("api/lists/{listId}/todos")]
        public async Task<IActionResult> CreateTodo(Guid listId, [FromBody] CreateItemViewModel todo)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var command = new CreateItem
            {
                AccountId = accountId,
                Email = userEmail,
                DueDate = todo.DueDate,
                ListId = listId,
                Name = todo.Name,
                Notes = todo.Notes
            };

            var todoItem = await _mediator.Send(command);

            return Ok(todoItem);
        }

        [HttpGet("api/lists/{listId}/todos")]
        public async Task<IActionResult> GetAllTodoItems(Guid listId)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorization = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorization.IsUserAuthorized())
            {
                var items = await _dapperQuery.GetAllTodoItemsAsync(listId);
                return Ok(items);
            }

            return Forbid();
        }

        [HttpGet("api/lists/{listId}/todos/{itemId}")]
        public async Task<IActionResult> GetTodoItemById(Guid listId, Guid itemId)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorization = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorization.IsUserAuthorized())
            {
                var item = await _dapperQuery.GetTodoItemByIdAsync(itemId);
                return Ok(item);
            }

            return Forbid();
        }

        [HttpPut("api/lists/{listId}/todos/{itemId}")]
        public async Task<IActionResult> EditTodo(Guid listId, Guid itemId, [FromBody] EditItemViewModel editItem)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var email = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var item = new EditItem()
            {
                AccountId = accountId,
                Email = email,
                ListId = listId,
                ItemId = itemId,
                Name = editItem.Name,
                Notes = editItem.Notes,
                DueDate = editItem.DueDate,
            };

            await _mediator.Send(item);
            return Ok();
        }

        [HttpPut("api/lists/{listId}/todos/{todoId}/completed")]
        public async Task<IActionResult> ToggleCompletedState(Guid listId, Guid todoId, [FromBody] ItemCompletedState itemCompletedState)
        {
            itemCompletedState.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            itemCompletedState.ItemId = todoId;

            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                await _mediator.Send(itemCompletedState);
                return Ok();
            }

            return Forbid();
        }

        [HttpGet("api/lists/{listId}/todos/{todoItemId}/layout")]
        public async Task<IActionResult> GetLayout(Guid listId, Guid todoItemId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var layout = await _dapperQuery.GetTodoItemLayoutAsync(todoItemId);
                return Ok(layout);
            }

            return Forbid();
        }

        [HttpPut("api/lists/{listId}/todos/{todoId}/layout")]
        public async Task<IActionResult> UpdateLayout(Guid listId, Guid todoId, [FromBody] ItemLayout itemLayout)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                itemLayout.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
                itemLayout.ItemId = todoId;
                await _mediator.Send(itemLayout);

                return Ok();
            }

            return Forbid();
        }

        [HttpDelete("api/lists/{listId}/todos/{todoId}")]
        public async Task<IActionResult> TrashItem(Guid listId, Guid todoId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

                var trashItem = new TrashItem
                {
                    AccountId = accountId,
                    ItemId = todoId
                };
                await _mediator.Send(trashItem);

                return Ok();
            }

            return Forbid();
        }

        [HttpGet("api/lists/itemsDueToday")]
        public async Task<IActionResult> GetItemsDueToday()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var date = DateTime.Now.Date;

            return Ok(await _dapperQuery.GetItemsBasedOnCurrentDate(accountId, date));
        }

        [HttpGet("api/lists/itemsWithDate")]
        public async Task<IActionResult> GetItemsWithDueDate()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            return Ok(await _dapperQuery.GetItemsThatHaveDueDate(accountId));
        }

        [HttpGet("api/lists/allItems")]
        public async Task<IActionResult> GetAllItems()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            return Ok(await _dapperQuery.GetAllItemsUnderAccount(accountId));
        }
    }
}