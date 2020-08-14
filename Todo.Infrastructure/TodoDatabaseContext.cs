using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Domain;
using Todo.Domain.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using Todo.Core;

namespace Todo.Infrastructure
{
    public class TodoDatabaseContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public TodoDatabaseContext()
        {
        }
        public TodoDatabaseContext(DbContextOptions<TodoDatabaseContext> options)
           : base(options)
        {
        }

        public TodoDatabaseContext(DbContextOptions<TodoDatabaseContext> options, IMediator mediator)
           : base(options)
        {
            _mediator = mediator;
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<TodoList> TodoLists { get; set; }
        public virtual DbSet<TodoListItem> TodoListItems { get; set; }
        public virtual DbSet<TodoListLayout> TodoListLayouts { get; set; }
        public virtual DbSet<SubItem> SubItems { get; set; }
        public virtual DbSet<SubItemLayout> SubItemLayouts { get; set; }
        public virtual DbSet<AccountPlan> AccountsPlans { get; set; }
        public virtual DbSet<AccountsLists> AccountsLists { get; set; }
        public virtual DbSet<RoleInvited> AccountsListsInvited { get; set; }
        public virtual DbSet<RoleDecline> AccountsListsDeclined { get; set; }
        public virtual DbSet<RoleContributor> AccountsListsContributor { get; set; }
        public virtual DbSet<RoleOwner> AccountsListsOwner { get; set; }
        public virtual DbSet<RoleLeft> AccountsListsLeft { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Payment> PaymentMethods { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var saveChanges = await base.SaveChangesAsync(cancellationToken);

            var domainEntities = ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents.Count > 0).ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent);

            return saveChanges;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {

                entity
                    .Property(e => e.Id).HasColumnName("ID");

                entity
                    .Property(e => e.FullName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity
                    .Property(e => e.CustomerId)
                    .HasColumnName("CustomerID");

                entity
                    .Property(e => e.PaymentMethodId)
                    .HasColumnName("PaymentMethodID");

                entity
                .Property(e => e.SubscriptionId)
                .HasColumnName("SubscriptionID");

            });

            modelBuilder.Entity<TodoList>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .HasColumnName("ID");

                entity
                    .Property(e => e.ListTitle)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity
                    .Property(e => e.Contributors).HasColumnName("Contributors")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<string>>(v));
            });

            modelBuilder.Entity<TodoListItem>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .HasColumnName("ID");

                entity
                    .Property(e => e.ListId)
                    .HasColumnName("ListID");

                entity
                    .Property(e => e.Name)
                    .HasColumnName("Name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity
                    .Property(e => e.Notes)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity
                    .Property(e => e.DueDate)
                    .HasColumnType("datetime");

            });

            modelBuilder.Entity<TodoListLayout>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .HasColumnName("ID");

                entity
                    .Property(e => e.ListId)
                    .HasColumnName("ListID");

                entity
                    .Property(e => e.Layout)
                    .HasColumnName("Layout")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<Guid>>(v)
                    );
            });

            modelBuilder.Entity<SubItemLayout>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .HasColumnName("ID");

                entity
                    .Property(e => e.ItemId);

                entity
                    .Property(e => e.Layout).HasColumnName("Layout")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<Guid>>(v)
                    );
            });

            modelBuilder.Entity<SubItem>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .HasColumnName("ID");

                entity
                    .Property(e => e.ListItemId)
                    .HasColumnName("ListItemID");

                entity
                    .Property(e => e.Name)
                    .HasColumnName("Name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity
                    .Property(e => e.Completed)
                    .HasColumnName("Completed");
            });

            modelBuilder.Entity<AccountsLists>()
                .ToTable("AccountsLists")
                .HasDiscriminator<byte>("Role")
                .HasValue<RoleOwner>(Roles.Owner)
                .HasValue<RoleContributor>(Roles.Contributer)
                .HasValue<RoleInvited>(Roles.Invited)
                .HasValue<RoleDecline>(Roles.Declined)
                .HasValue<RoleLeft>(Roles.Left);

            modelBuilder.Entity<AccountPlan>(entity =>
            {
                entity
                    .HasKey(e => e.Id);

                entity
                    .Property(e => e.AccountId)
                    .HasColumnName("AccountID");

                entity
                    .Property(e => e.PlanId)
                    .HasColumnName("PlanID");

                entity
                    .Property(e => e.ListCount)
                    .HasColumnName("ListCount");
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .HasColumnName("ID");

                entity
                    .Property(e => e.Name)
                    .HasColumnName("Name");

                entity
                    .Property(e => e.MaxContributors)
                    .HasColumnName("MaxContributors");

                entity
                    .Property(e => e.MaxLists)
                    .HasColumnName("MaxLists");

                entity
                    .Property(e => e.CanNotifyViaEmail)
                    .HasColumnName("CanNotifyViaEmail");

                entity
                    .Property(e => e.CanAddDueDates)
                    .HasColumnName("CanAddDueDates");
            });

            modelBuilder.Entity<Payment>(entity =>
           {
               entity
                   .HasKey(e => e.TokenId);

               entity
                   .Property(e => e.AccountId)
                   .HasColumnName("AccountID");
           });
        }
    }
}