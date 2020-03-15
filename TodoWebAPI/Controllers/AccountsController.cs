﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoWebAPI.Models;
using TodoWebAPI.Presentation;

namespace TodoWebAPI.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly IConfiguration _config;
        private ContextService _contextService;

        public AccountsController(ToDoContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _contextService = new ContextService(_context, _config);
        }

        [HttpPost("accounts")]
        public IActionResult CreateAccount(CreateAccountModel account)
        {
            var a = new Accounts();
            var usernameExists = _context.Accounts.Where(x => x.UserName == account.UserName).FirstOrDefault() != null;

            if (usernameExists)
            {
                return BadRequest("Username needed.");
            }
            else if (account.Password == null)
            {
                return BadRequest("Password needed.");
            }

            a.FullName = account.FullName;
            a.UserName = account.UserName;
            a.Password = account.Password;

            _context.Accounts.Add(a);

            _context.SaveChanges();

            account.Id = a.Id;

            if (account.Picture != null)
            {
                var image = new ImageHandler(connectionString: _config.GetConnectionString("Development"));

                image.StoreImageProfile(account);
            }

            return Ok($"{account.UserName} was created.");
        }

        [HttpGet("accounts/{accountId}")]
        public IActionResult GetAccount(int accountId)
        {
            var account = _context.Accounts.Find(accountId);
            var accountPicture = "";

            if (account == null)
            {
                return NotFound("Profile doesn't exist. :(");
            }
            else if (account.Picture != null)
            {
                accountPicture = Convert.ToBase64String(account.Picture);
            }

            var profilePresentation = new AccountPresentation()
            {
                Id = account.Id,
                FullName = account.FullName,
                UserName = account.UserName,
                Picture = accountPicture,
            };

            return Ok(profilePresentation);
        }

        [HttpDelete("accounts/{accountId}")]
        public IActionResult DeleteAccount(int accountId)
        {
            return NotFound();
        }

        [HttpPost("accounts/{accountId}/lists")]
        public IActionResult CreateList(int accountId, [FromBody] string title)
        {
            if(_contextService.AccountExists(accountId))
            {
                if(title == "")
                {
                    title = "Untitled List";
                }

                var list = new Lists()
                {
                    AccountId = accountId,
                    ListTitle = title
                };

                _context.Lists.Add(list);
                _context.SaveChanges();
                return Ok( new { list.Id, list.ListTitle } );
            }
            return NotFound("Account doesn't exist.");
        }

        [HttpGet("accounts/{accountId}/lists")]
        public IActionResult GetLists(int accountId)
        {
            if(_contextService.AccountExists(accountId))
            {
                var listsFromDatabase = _context.Lists.Where(l => l.AccountId == accountId).ToList();
                var lists = new List<ListPresentation>();

                foreach(var list in listsFromDatabase)
                {
                    lists.Add(new ListPresentation(list));
                }

                return Ok(lists);
            }
            return NotFound("Account doesn't exist.");
        }

        [HttpPut("accounts/{accountId}/lists/{listId}")]
        public IActionResult UpdateList(int accountId, int listId, [FromBody] string title)
        {
            var list = _context.Lists.Find(listId);

            if(list != null)
            {
                if (list.AccountId != accountId)
                {
                    return BadRequest("List doesn't belong to user.");
                }

                if (title == "")
                {
                    title = "Untitled List";
                }
                list.ListTitle = title;

                _context.Lists.Update(list);
                _context.SaveChanges();
                return Ok(new { list.Id, list.ListTitle });
            }
            return NotFound("List doesn't exist.");
        }

        [HttpDelete("accounts/{accountId}/lists/{listId}")]
        public IActionResult DeleteList(int accountId, int listId)
        {
            var list = _context.Lists.Find(listId);

            if (list != null)
            {
                if (list.AccountId != accountId)
                {
                    return BadRequest("List doesn't belong to user.");
                }

                _contextService.RemoveList(list);

                return Ok("List deleted");
            }
            return NotFound("List doesn't exist.");
        }

        [HttpPost("accounts/{accountId}/todos")]
        public IActionResult CreateTodo(int accountId)
        {
            return NotFound();
        }

        [HttpPut("accounts/{accountId}/todos/{todoId}")]
        public IActionResult EditTodo(int accountId, int todoId)
        {
            return NotFound();
        }

        [HttpDelete("accounts/{accountId}/todos/{todoId}")]
        public IActionResult DeleteTodo(int accountId, int todoId)
        {
            return NotFound();
        }
    }
}
