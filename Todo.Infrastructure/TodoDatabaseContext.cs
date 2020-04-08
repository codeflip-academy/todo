using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Domain;
using Todo.Domain.Repositories;

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
                entity.HasIndex(e => e.UserName)
                    .HasName("UQ__Accounts__C9F2845640260CAD")
                    .IsUnique();
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TodoList>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.ListTitle)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TodoListItem>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ListId).HasColumnName("ListID");

                entity.Property(e => e.Notes)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.ToDoName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
