﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public SubItemController(IMediator mediator, IConfiguration config)
        {
            _mediator = mediator;
            _config = config;
        }

        [HttpGet("api/lists/{listId}/todos/{todoItemId}/subitems")]
        public async Task<IActionResult> GetSubItems(int todoItemId)
        {
            var dapper = new DapperQuery(_config);

            var subItems = await dapper.GetSubItems(todoItemId);

            return Ok(subItems);
        }

        [HttpPost("api/lists/{listId}/todos/{todoItemId}/subitems")]
        public async Task<IActionResult> CreateSubItem(int listId, int todoItemId, [FromBody] CreateSubItem createSubItem)
        {
            createSubItem.AccountId = User.ReadClaimAsIntValue("urn:codefliptodo:accountid");
            createSubItem.ListId = listId;
            createSubItem.ListItemId = todoItemId;
            var subItem = await _mediator.Send(createSubItem);

            return Ok(subItem);
        }

        [HttpPut("api/subitems/{subitemId}/completed")]
        public async Task<IActionResult> ToggleCompletedState(int subItemId, [FromBody] bool completed)
        {
            var accountId = User.ReadClaimAsIntValue("urn:codefliptodo:accountid");

            var subItemCompleted = new SubItemCompletedState
            {
                AccountId = accountId,
                SubItemId = subItemId,
                Completed = completed
            };
            await _mediator.Send(subItemCompleted);
            return Ok();
        }

        [HttpPut("api/subitems/{subitemId}")]
        public async Task<IActionResult> UpdateSubItem(int subitemId, [FromBody] EditSubItem editSubItem)
        {
            var accountId = User.ReadClaimAsIntValue("urn:codefliptodo:accountid");

            editSubItem.AccountId = accountId;
            editSubItem.SubItemId = subitemId;

            await _mediator.Send(editSubItem);

            return Ok();
        }

        [HttpDelete("api/subitems/{subitemId}")]
        public async Task<IActionResult> TrashSubItem(int subitemId)
        {
            var accountId = User.ReadClaimAsIntValue("urn:codefliptodo:accountid");

            var trashSubItem = new TrashSubItem
            {
                AccountId = accountId,
                SubItemId = subitemId
            };

            await _mediator.Send(trashSubItem);

            return Ok("Subitem deleted!!!");
        }
    }
}