using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using Todo.Infrastructure.EFRepositories;
using Todo.Infrastructure.Email;

namespace TodoWebAPI.CronJob
{
    public class SubscriptionDowngradeJob : CronJobService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DueDateJob> _logger;
        private readonly IEmailService _emailService;
        private readonly DapperQuery _dapperQuery;

        public SubscriptionDowngradeJob(
            IConfiguration configuration,
            IScheduleConfig<DueDateJob> config,
            ILogger<DueDateJob> logger,
            IEmailService emailService,
            DapperQuery dapperQuery)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _dapperQuery = dapperQuery;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscription downgrade job runing");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            var downgrades = await _dapperQuery.GetDowngradesAsync();
            var dbContext = new TodoDatabaseContext();

            foreach (var downgrade in downgrades)
            {
                if (downgrade.BiliingCycleEnd.Date >= DateTime.Now.Date)
                {
                    //delete lists and update plan and decrement list couint and decrement contribuotrs list count
                }
            }


            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Subscription downgrade job runing");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscription downgrade job running");
            return base.StopAsync(cancellationToken);
        }
    }
}
