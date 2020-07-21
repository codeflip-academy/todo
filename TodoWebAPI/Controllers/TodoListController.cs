using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoWebAPI.Models;
using TodoWebAPI.Presentation;
using MediatR;
using TodoWebAPI.UserStories.ListLayout;
using Microsoft.AspNetCore.Authorization;
using TodoWebAPI.Extentions;
using TodoWebAPI.UserStories.SendInvitation;
using Todo.Domain;
using Todo.Domain.Repositories;
using TodoWebAPI.UserStories;

namespace TodoWebAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class TodoListController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly DapperQuery _dapperQuery;
        private readonly IPlanRepository _planRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;

        public TodoListController(
            IMediator mediator,
            DapperQuery dapperQuery,
            IPlanRepository planRepository,
            IAccountPlanRepository accountPlanRepository)
        {
            _mediator = mediator;
            _dapperQuery = dapperQuery;
            _planRepository = planRepository;
            _accountPlanRepository = accountPlanRepository;
        }

        [HttpPost("api/lists")]
        public async Task<IActionResult> CreateList(CreateList createTodoList)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(User.ReadClaimAsGuidValue("urn:codefliptodo:accountid"));
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);

            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            if (!accountPlanAuthorization.CanCreateList())
                return BadRequest("Reached maximum number of lists allowed on your plan.");

            createTodoList.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var todoList = await _mediator.Send(createTodoList);

            if (todoList == null)
                return BadRequest("Unable to create list :(");

            return Ok();
        }

        [HttpGet("api/lists")]
        public async Task<IActionResult> GetLists()
        {
            var lists = await _dapperQuery.GetListsAsync(User.ReadClaimAsGuidValue("urn:codefliptodo:accountid"));
            var accountContributors = await _dapperQuery.GetContributorsAsync(User.ReadClaimAsGuidValue("urn:codefliptodo:accountid"));
            var todoListsPresentation = new TodoListsPresentation(lists, accountContributors);

            return Ok(todoListsPresentation);
        }

        [HttpGet("api/lists/{listId}")]
        public async Task<IActionResult> GetList(Guid listId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                return Ok(list);
            }

            return Forbid();
        }

        [HttpPut("api/lists/{listId}")]
        public async Task<IActionResult> UpdateList(Guid listId, UpdateList updatedList)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            updatedList.ListId = listId;
            updatedList.Email = userEmail;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var mediator = await _mediator.Send(updatedList);
                if(mediator == null)
                {
                    return BadRequest("Can't rename list because you're not an owner.");
                }
                return Ok();
            }

            return Forbid();
        }

        [HttpDelete("api/lists/{listId}")]
        public async Task<IActionResult> DeleteList(Guid listId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var deleteTodoModel = new DeleteList();

            deleteTodoModel.Email = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            deleteTodoModel.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            deleteTodoModel.ListId = listId;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                await _mediator.Send(deleteTodoModel);
                return Ok();
            }

            return Forbid();
        }

        [HttpPut("api/lists/{listId}/layout")]
        public async Task<IActionResult> UpdateLayout(Guid listId, [FromBody] ListLayout todoListLayoutModel)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            todoListLayoutModel.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            todoListLayoutModel.ListId = listId;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                await _mediator.Send(todoListLayoutModel);
                return Ok();
            }

            return Forbid();
        }

        [HttpGet("api/lists/{listId}/layout")]
        public async Task<IActionResult> GetTodoListLayout(Guid listId)
        {
            var userEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var list = await _dapperQuery.GetListAsync(listId);

            var todoListAuthorizationValidator = new TodoListAuthorizationValidator(list.Contributors, userEmail);

            if (todoListAuthorizationValidator.IsUserAuthorized())
            {
                var layout = await _dapperQuery.GetTodoListLayoutAsync(listId);
                return Ok(layout.Layout);
            }

            return Forbid();
        }

        [HttpPost("api/lists/{listId}/email")]
        public async Task<IActionResult> SendInvitaion(Guid listId, [FromBody] SendInvitationViewModel invitation)
        {
            var senderAccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var senderEmail = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var command = new SendInvitation()
            {
                SenderAccountId = senderAccountId,
                SenderEmail = senderEmail,
                ListId = listId,
                InviteeEmail = invitation.Email
            };

            var response = await _mediator.Send(command);

            if (response == true)
                return Ok();

            return BadRequest();
        }

        [HttpPost("api/lists/{listId}/accept")]
        public async Task<IActionResult> AcceptInvitation(Guid listId)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(User.ReadClaimAsGuidValue("urn:codefliptodo:accountid"));
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            var command = new AcceptInvitaion()
            {
                AccountId = accountId,
                ListId = listId
            };

            if (!accountPlanAuthorization.CanCreateList())
                return BadRequest("Reached maximum number of lists allowed on your plan.");

            var response = await _mediator.Send(command);

            return Ok();
        }

        [HttpPost("api/lists/{listId}/decline")]
        public async Task<IActionResult> DeclineInvitation(Guid listId)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var command = new DeclineInvitation()
            {
                AccountId = accountId,
                ListId = listId
            };

            await _mediator.Send(command);

            return Ok();
        }

        [HttpPost("api/lists/{listId}/removeSelf")]
        public async Task<IActionResult> RemoveSelfFromlist(Guid listId)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var command = new RemoveSelfFromList()
            {
                AccountId = accountId,
                ListId = listId,
            };

            await _mediator.Send(command);

            return Ok();
        }

        [HttpPost("api/lists/{listId}/email/removeContributor")]
        public async Task<IActionResult> RemoveContributorFromList(Guid listId, string email)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var command = new RemoveContributorFromList
            {
                AccountId = accountId,
                ListId = listId,
                Email = email
            };

            await _mediator.Send(command);

            return Ok();
        }
    }
}
