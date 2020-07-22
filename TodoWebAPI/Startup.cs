using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using Todo.Infrastructure.EFRepositories;
using Todo.Infrastructure.Email;
using TodoWebAPI.Data;
using TodoWebAPI.CronJob;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Octokit;
using Octokit.Internal;
using TodoWebAPI.Models;
using Todo.Infrastructure.Guids;
using Dapper;
using TodoWebAPI.TypeHandlers;
using TodoWebAPI.SignalR;
using Microsoft.AspNetCore.SignalR;
using TodoWebAPI.ServiceBus;

namespace TodoWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "Policy";
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEmailService, SendGridEmailService>();

            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsTelemetryWorkerService();

            services.AddDbContext<TodoDatabaseContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Development"))
            );
            services.AddScoped<ITodoListRepository, EFTodoListRepository>();
            services.AddScoped<IAccountsListsRepository, EFAccountsListsRepository>();
            services.AddScoped<ITodoListLayoutRepository, EFTodoListLayoutRepository>();
            services.AddScoped<ITodoListItemRepository, EFTodoListItemRepository>();
            services.AddScoped<IAccountRepository, EFAccountRepository>();
            services.AddScoped<IAccountPlanRepository, EFAccountPlanRepository>();
            services.AddScoped<IPlanRepository, EFPlanRepository>();
            services.AddScoped<ISubItemRepository, EFSubItemRepository>();
            services.AddScoped<ISubItemLayoutRepository, EFSubItemLayout>();
            services.AddSingleton<DapperQuery>();
            services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();
            services.AddScoped<ISequentialIdGenerator, SequentialIdGenerator>();
            services.AddControllers();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.AddSingleton<IServiceBusConsumer, ServiceBusConsumer>();
            services.AddSingleton<IEmailQueue, AzureServiceBusEmailQueue>();

            services.AddCronJob<DueDateJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"00 12 * * *";
            });


            services.AddSignalR();

            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
                .AddCookie()
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = Configuration["GitHub:ClientId"];
                    options.ClientSecret = Configuration["GitHub:ClientSecret"];
                    options.CallbackPath = new PathString("/external-callback");
                    options.Scope.Add("user:email");

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.SaveTokens = true;

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey("urn:github:login", "login");
                    options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            context.RunClaimActions(user.RootElement);

                            var github = new GitHubClient(new Octokit.ProductHeaderValue("codefliptodo"), new InMemoryCredentialStore(new Credentials(context.AccessToken)));
                            var primaryEmail = (await github.User.Email.GetAll()).FirstOrDefault(email => email.Primary && email.Verified).Email;

                            // call repo to create or get account
                            var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();

                            var account = await mediator.Send(new CreateAccountModel
                            {
                                Email = primaryEmail,
                                FullName = context.Identity.Claims.First(c => c.Type == ClaimTypes.Name)?.Value,
                                PictureUrl = context.Identity.Claims.First(c => c.Type == "urn:github:avatar").Value
                            });

                            context.Identity.AddClaim(new Claim("urn:codefliptodo:accountid", account.Id.ToString()));

                            context.Identity.AddClaim(new Claim(
                            ClaimTypes.Email, primaryEmail,
                            ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        }
                    };
                });

            // Register type handlers for Dapper
            SqlMapper.AddTypeHandler(typeof(List<string>), new JsonObjectTypeHandler());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseFileServer();

            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notifications");
            });

            var bus = app.ApplicationServices.GetRequiredService<IServiceBusConsumer>();
            bus.RegisterOnMessageHandlerAndReceiveMessages();
        }
    }
}