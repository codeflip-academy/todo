using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Todo.Infrastructure;
using TodoWebAPI.Extentions;
using TodoWebAPI.UserStories.DeleteAccount;
using TodoWebAPI.UserStories.EmailFilter;
using TodoWebAPI.UserStories.RoleChanges;

namespace TodoWebAPI.Controllers
{
    [ApiController]

    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TodoDatabaseContext _todoDatabaseContext;
        private readonly DapperQuery _dapper;
        private readonly IConfiguration _config;
        public AccountsController(IConfiguration config,
            IMediator mediator, TodoDatabaseContext todoDatabaseContext,
            DapperQuery dapper)
        {
            _config = config;
            _mediator = mediator;
            _todoDatabaseContext = todoDatabaseContext;
            _dapper = dapper;
        }

        [AllowAnonymous]
        [HttpGet("api/accounts/login")]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.FindFirst(ClaimTypes.Email).Value;
                return Ok(User.FindFirst(c => c.Type == "urn:github:avatar").Value);
            }
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [Authorize]
        [HttpGet("api/accounts/logout")]
        public async Task Logout(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("api/accounts")]
        public async Task<IActionResult> GetAccount()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var dapper = new DapperQuery(_config);

            var account = await dapper.GetAccountAsync(accountId);

            return Ok(account);
        }

        [Authorize]
        [HttpGet("api/accounts/contributors")]
        public async Task<IActionResult> GetContributors()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");
            var dapper = new DapperQuery(_config);
            var contributors = await dapper.GetContributorsAsync(accountId);

            return Ok(contributors);
        }

        [Authorize]
        [HttpDelete("api/accounts")]
        public async Task<IActionResult> DeleteAccountAsync()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var deleteAccount = new DeleteAccount
            {
                AccountId = accountId
            };

            await _mediator.Send(deleteAccount);

            await _todoDatabaseContext.SaveChangesAsync();

            return Ok("account deleted!");
        }

        [Authorize]
        [HttpGet("api/accounts/plan")]
        public async Task<IActionResult> GetPlanAsync()
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            var dapper = new DapperQuery(_config);

            return Ok(await dapper.GetPlanByAccountIdAsync(accountId));
        }

        [Authorize]
        [HttpPut("api/accounts/role")]
        public async Task<IActionResult> ChangeRoleAysnc([FromBody] ChangePlan roleChanged)
        {
            var accountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            roleChanged.AccountId = accountId;

            var mediator = await _mediator.Send(roleChanged);

            if (mediator == false)
            {
                return BadRequest();
            }
            return Ok();
        }

        [Authorize]
        [HttpPut("api/accounts/emailFilter")]
        public async Task<IActionResult> FilterEmailAsync([FromBody] EmailFilter emailFilter)
        {
            emailFilter.AccountId = User.ReadClaimAsGuidValue("urn:codefliptodo:accountid");

            await _mediator.Send(emailFilter);

            return Ok();
        }

        [Authorize]
        [HttpGet("api/accounts/emailFilter")]
        public async Task<IActionResult> GetEmailFilter()
        {
            var accountId = Guid.Parse(User.FindFirst(c => c.Type == "urn:codefliptodo:accountid").Value);

            return Ok(await _dapper.GetEmailFilterAsync(accountId));
        }
    }
}
