using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Todo.Domain;
using TodoWebAPI.ApplicationServices;
using TodoWebAPI.Extentions;
using TodoWebAPI.Models;
using TodoWebAPI.UserStories.CreateSubItem;
using TodoWebAPI.UserStories.EditSubItem;
using TodoWebAPI.UserStories.SubItemCompletedState;
using TodoWebAPI.UserStories.TrashSubItem;

namespace TodoWebAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class SubItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;
        private readonly DapperQuery _dapperQuery;

        public SubItemController(
            IMediator mediator,
            IConfiguration config,
            DapperQuery dapperQuery)
        {
            _mediator = mediator;
            _config = config;
            _dapperQuery = dapperQuery;
        }

        [HttpGet("api/lists/{listId}/todos/{todoItemId}/subitems")]
        public async Task<IActionResult> GetSubItems(Guid listId, Guid todoItemId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var subItems = await _dapperQuery.GetSubItems(todoItemId);

                return Ok(subItems);
            }

            return Forbid();
        }

        [HttpPost("api/lists/{listId}/todos/{todoItemId}/subitems")]
        public async Task<IActionResult> CreateSubItem(Guid listId, Guid todoItemId, [FromBody] CreateSubItem createSubItem)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                createSubItem.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
                createSubItem.ListId = listId;
                createSubItem.ListItemId = todoItemId;
                var subItem = await _mediator.Send(createSubItem);

                return Ok(subItem);
            }

            return Forbid();
        }

        [HttpPut("api/lists/{listId}/todos/{todoItemId}/subitems/{subitemId}/completed")]
        public async Task<IActionResult> ToggleCompletedState(Guid listId, Guid subItemId, [FromBody] bool completed)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

                var subItemCompleted = new SubItemCompletedState
                {
                    AccountId = accountId,
                    SubItemId = subItemId,
                    Completed = completed
                };
                await _mediator.Send(subItemCompleted);
                return Ok();
            }

            return Forbid();
        }

        [HttpPut("api/lists/{listId}/todos/{todoItemId}/subitems/{subitemId}")]
        public async Task<IActionResult> UpdateSubItem(Guid listId, Guid subitemId, [FromBody] EditSubItem editSubItem)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

                editSubItem.AccountId = accountId;
                editSubItem.SubItemId = subitemId;

                await _mediator.Send(editSubItem);

                return Ok();
            }

            return Forbid();
        }

        [HttpDelete("api/lists/{listId}/todos/{todoItemId}/subitems/{subitemId}")]
        public async Task<IActionResult> TrashSubItem(Guid listId, Guid subitemId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

                var trashSubItem = new TrashSubItem
                {
                    AccountId = accountId,
                    SubItemId = subitemId
                };

                await _mediator.Send(trashSubItem);

                return Ok("Subitem deleted!!!");
            }

            return Forbid();
        }
    }
}