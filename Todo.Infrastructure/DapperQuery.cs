using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Todo.Domain;
using System.Data.SqlClient;
using Todo.Infrastructure.Dto;
using Dapper;
using Dapper.Transaction;
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
        public async Task<AccountDto> GetAccountAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<AccountDto>("SELECT * From Accounts Where ID = @accountId", new { accountId = accountId });

                return result.FirstOrDefault();
            }
        }

        public async Task<Dictionary<string, AccountContributorsDto>> GetContributorsAsync(Guid accountId)
        {
            Dictionary<string, AccountContributorsDto> contributors = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var contributorsResult = await connection.QueryAsync<AccountContributorsDto>(@"
                    select distinct a.FullName, a.PictureUrl, a.Email
                    from AccountsLists al
                    INNER JOIN (select ListID from AccountsLists where AccountID = @accountId)
                    al2 ON al.ListID = al2.ListID
                    inner join Accounts a on a.ID = al.AccountID", new { accountId = accountId });

                contributors = contributorsResult.ToDictionary(kvp => kvp.Email, kvp => kvp);

                return contributors;
            }
        }

        public async Task<List<TodoListItemDto>> GetAllTodoItemsAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TodoListItemDto>("SELECT * From TodoListItems WHERE ListID = @listId", new { listId = listId });

                return result.ToList();
            }
        }
        public async Task<TodoListItemDto> GetTodoItemByIdAsync(Guid itemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TodoListItemDto>("SELECT * From TodoListItems WHERE ID = @id", new { id = itemId });
                var item = result.FirstOrDefault();

                return item;
            }
        }

        public async Task<TodoListDto> GetListAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoListDto>("SELECT * FROM TodoLists WHERE ID = @listId", new { listId = listId });
                return result.FirstOrDefault();
            }
        }

        public async Task<PlanDto> GetPlanByAccountIdAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<PlanDto>("Select Name, MaxContributors, MaxContributors, MaxLists, CanAddDueDates From Plans Where ID = (Select PlanID From AccountsPlans Where AccountID = @accountId)", new { accountId = accountId });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<TodoListDto>> GetListsAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoListDto>(@"
                    SELECT t.ID, t.ListTitle, a.AccountID, t.Completed, t.Contributors, a.Role, (SELECT COUNT(*) FROM TodoListItems where Completed = 0 and TodoListItems.ListID = t.ID) as IncompleteCount
                    FROM TodoLists as t INNER JOIN AccountsLists as a
                    ON t.ID = a.ListID WHERE a.AccountID = @accountId
                    AND NOT (a.Role = @left OR a.Role = @declined)",
                    new { accountId = accountId, left = Roles.Left, declined = Roles.Declined });
                return result.ToList();
            }
        }

        public async Task<TodoListLayoutDto> GetTodoListLayoutAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoListLayoutDto>("SELECT * FROM TodoListLayouts WHERE ListId = @listId", new { listId = listId });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<TodoListItem>> GetUncompletedItemsByListIdAsync(Guid listId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoListItem>("Select * from TodoListItems Where Completed = 0 and ListID = @listId", new { listId = listId });
                return result.ToList();
            }
        }


        public async Task<TodoItemLayoutDto> GetTodoItemLayoutAsync(Guid itemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<TodoItemLayoutDto>("SELECT Layout FROM SubItemLayouts WHERE ItemId = @itemId", new { itemId = itemId });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<SubItemDto>> GetSubItems(Guid listItemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<SubItemDto>("SELECT * FROM SubItems WHERE ListItemID = @listItemId", new { listItemId = listItemId });

                return result.ToList();
            }
        }

        public async Task<List<TodoListItemDto>> GetItemsFromListItemsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<TodoListItemDto>("SELECT * FROM TodoListItems WHERE DueDate IS NOT NULL");
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

        public async Task<List<DowngradeDto>> GetDowngradesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<DowngradeDto>("SELECT AccountID, BillingCycleEnd, PlanID From Downgrades");
                return result.ToList();
            }
        }
    }
}
