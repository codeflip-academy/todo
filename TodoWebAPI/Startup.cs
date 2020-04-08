using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using Todo.Infrastructure.EFRepositories;
using Todo.Infrastructure.Email;
using TodoWebAPI.Data;
using Todo.WebAPI.ApplicationServices;
using Todo.Infrastructure.ServiceBus;

namespace TodoWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "Policy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Policy", builder => builder
                    .WithOrigins("http://localhost:5002")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });

            services.AddDbContext<TodoDatabaseContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Development"))
            );
            services.AddScoped<ITodoListRepository, EFTodoListRepository>();
            services.AddScoped<ITodoListItemRepository, EFTodoListItemRepository>();
            services.AddScoped<IAccountRepository, EFAccountRepository>();
            services.AddSingleton<IEmailService, DebuggerWindowOutputEmailService>();
            services.AddScoped<IAccountProfileImageRepository, AccountProfileImageRepository>((x) => new AccountProfileImageRepository(Configuration.GetConnectionString("Development")));
            services.AddScoped<TodoListApplicationService>();
            services.AddScoped<TodoListItemApplicationService>();
            services.AddScoped<AccountsApplicationService>();
            services.AddScoped<IServiceBusEmail, ServiceBusEmail>();
            services.AddControllers();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
