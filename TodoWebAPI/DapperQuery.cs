using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Todo.Domain;
using System.Data.SqlClient;
using TodoWebAPI.Presentation;
using Dapper;
using Dapper.Transaction;
using TodoWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using Todo.Infrastructure;

namespace TodoWebAPI
{
    public class DapperQuery
    {
        private readonly string _connectionString;

        public DapperQuery(IConfiguration config)
        {
            _connectionString = config.GetSection("ConnectionStrings")["Development"];
        }
        public async Task<AccountPresentation> GetAccountAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<AccountPresentation>("SELECT * From Accounts Where ID = @accountId", new { accountId = accountId });

                return result.FirstOrDefault();
            }
        }

        public async Task<Dictionary<string, AccountContributorsPresentation>> GetContributorsAsync(Guid accountId)
        {
            Dictionary<string, AccountContributorsPresentation> contributors = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var contributorsResult = await connection.QueryAsync<AccountContributorsPresentation>(@"
                    select distinct a.FullName, a.PictureUrl, a.Email
                    from AccountsLists al
                    INNER JOIN (select ListID from AccountsLists where AccountID = @accountId)
                    al2 ON al.ListID = al2.ListID
                    inner join Accounts a on a.ID = al.AccountID", new { accountId = accountId });

                contributors = contributorsResult.ToDictionary(kvp => kvp.Email, kvp => kvp);

                return contributors;
            }
        }

        public async Task<List<TodoListItemModel>> GetAllTodoItemsAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TodoListItemModel>("SELECT * From TodoListItems WHERE ListID = @listId", new { listId = listId });

                return result.ToList();
            }
        }

        public async Task<TodoListModel> GetListAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoListModel>("SELECT * FROM TodoLists WHERE ID = @listId", new { listId = listId });
                return result.FirstOrDefault();
            }
        }

        public async Task<PlanPresentation> GetPlanByAccountIdAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<PlanPresentation>("Select Name, MaxContributors, MaxContributors, MaxLists, CanAddDueDates From Plans Where ID = (Select PlanID From AccountsPlans Where AccountID = @accountId)", new { accountId = accountId });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<TodoListModel>> GetListsAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TodoListModel>(@"
                    SELECT t.ID, t.ListTitle, a.AccountID, t.Completed, t.Contributors, a.Role
                    FROM TodoLists as t INNER JOIN AccountsLists as a
                    ON t.ID = a.ListID WHERE a.AccountID = @accountId
                    AND NOT (a.Role = @left OR a.Role = @declined)",
                    new { accountId = accountId, left = Roles.Left, declined = Roles.Declined });

                return result.ToList();
            }
        }

        public async Task<TodoListLayoutPresentation> GetTodoListLayoutAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoListLayoutPresentation>("SELECT * FROM TodoListLayouts WHERE ListId = @listId", new { listId = listId });
                return result.FirstOrDefault();
            }
        }

        public async Task<TodoItemLayoutPresentation> GetTodoItemLayoutAsync(Guid itemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoItemLayoutPresentation>("SELECT Layout FROM SubItemLayouts WHERE ItemId = @itemId", new { itemId = itemId });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<SubItemModel>> GetSubItems(Guid listItemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<SubItemModel>("SELECT * FROM SubItems WHERE ListItemID = @listItemId", new { listItemId = listItemId });

                return result.ToList();
            }
        }

        public async Task<List<TodoListItem>> GetItemsFromListItemsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TodoListItem>("SELECT * FROM TodoListItems WHERE DueDate IS NOT NULL");
                return result.ToList();
            }
        }

        public async Task<List<string>> GetEmailsFromAccountsByListIdAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<string>("SELECT Email FROM Accounts as a INNER JOIN AccountsLists as l ON a.ID = l.AccountID and l.ListID = @listId", new { listId = listId });
                return result.ToList();
            }
        }

        public async Task<Guid> GetAccountIdByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<Guid>("SELECT ID FROM Accounts WHERE Email = @email", new { email = email });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<string>> GetContributorsByListIdAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<string>("SELECT Contributors FROM TodoLists WHERE Id = @listId", new { listId = listId });
                return result.ToList();
            }
        }
    }
}
