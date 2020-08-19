using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using Todo.Infrastructure.Dto;
using Todo.Infrastructure.EFRepositories;
using Todo.Infrastructure.Email;
using TodoWebAPI.UserStories;

namespace TodoWebAPI.CronJob
{
    public class SubscriptionDowngradeJob : CronJobService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SubscriptionDowngradeJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionDowngradeJob(
            IConfiguration configuration,
            IScheduleConfig<SubscriptionDowngradeJob> config,
            ILogger<SubscriptionDowngradeJob> logger,
            IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo, serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscription downgrade job running");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken, IServiceProvider serviceProvider)
        {
            var todoDatabaseContext = serviceProvider.GetRequiredService<TodoDatabaseContext>();
            var accountPlanRepository = serviceProvider.GetRequiredService<IAccountPlanRepository>();
            var accountsListsRepository = serviceProvider.GetRequiredService<IAccountsListsRepository>();
            var listRepository = serviceProvider.GetRequiredService<ITodoListRepository>();
            var planRepository = serviceProvider.GetRequiredService<IPlanRepository>();
            var downgradeRepository = serviceProvider.GetRequiredService<IDowngradeRepository>();
            var accountRepository = serviceProvider.GetRequiredService<IAccountRepository>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            foreach (var downgrade in await downgradeRepository.GetDowngradesAsync())
            {
                var accountPlan = await accountPlanRepository.FindAccountPlanByAccountIdAsync(downgrade.AccountId);

                accountPlan.ChangePlan(downgrade.PlanId);

                downgrade.Downgraded();
                await downgradeRepository.SaveChangesAsync();

                _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Subscription downgrade job running");
            }
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscription downgrade job running");
            return base.StopAsync(cancellationToken);
        }
    }
}