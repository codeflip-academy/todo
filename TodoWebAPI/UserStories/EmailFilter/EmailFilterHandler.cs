using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories.EmailFilter
{
    public class EmailFilterHandler : AsyncRequestHandler<EmailFilter>
    {
        private readonly IAccountRepository _account;

        public EmailFilterHandler(IAccountRepository account)
        {
            _account = account;
        }
        protected async override Task Handle(EmailFilter request, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(request.AccountId);

            account.FilterEmails(request.EmailDueDate, request.EmailListCompleted, request.EmailItemCompleted, request.EmailInvitation);
            await _account.SaveChangesAsync();
        }
    }
}