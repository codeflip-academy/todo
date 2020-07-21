using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Repositories;
using Todo.Domain.DomainEvents;
using Todo.Domain;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories.SendInvitation
{
    public class SendInvitationUserStory : IRequestHandler<SendInvitation, bool>
    {
        private readonly ITodoListRepository _todoListRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IAccountsListsRepository _accountsListsRepository;

        public SendInvitationUserStory(
            ITodoListRepository todoListRepository,
            IAccountRepository accountRepository,
            IAccountPlanRepository accountPlanRepository,
            IPlanRepository planRepository,
            IAccountsListsRepository accountsListsRepository)
        {
            _todoListRepository = todoListRepository;
            _accountRepository = accountRepository;
            _accountPlanRepository = accountPlanRepository;
            _planRepository = planRepository;
            _accountsListsRepository = accountsListsRepository;
        }

        public async Task<bool> Handle(SendInvitation request, CancellationToken cancellationToken)
        {
            var accountPlan = await _accountPlanRepository.FindAccountPlanByAccountIdAsync(request.SenderAccountId);
            var plan = await _planRepository.FindPlanByIdAsync(accountPlan.PlanId);
            var list = await _todoListRepository.FindTodoListIdByIdAsync(request.ListId);
            var accountsLists = await _accountsListsRepository.FindAccountsListsByAccountIdAsync(request.SenderAccountId, request.ListId);
            var accountPlanAuthorization = new AccountPlanAuthorizationValidator(accountPlan, plan);

            if (accountsLists.UserIsOwner(request.SenderAccountId))
            {
                if (accountPlanAuthorization.CanAddContributor(list))
                {
                    var invitee = await _accountRepository.FindAccountByEmailAsync(request.InviteeEmail);
                    var inviteeAccountsListsLeft = await _accountsListsRepository.FindAccountsListsLeftByAccountIdAsync(invitee.Id, request.ListId);
                    var inviteeAccountsListsDeclined = await _accountsListsRepository.FindAccountsListsDeclinedByAccountIdAsync(invitee.Id, request.ListId);
                    var inviteeAccountsLists = await _accountsListsRepository.FindAccountsListsByAccountIdAsync(invitee.Id, request.ListId);

                    if(inviteeAccountsLists != null && inviteeAccountsLists.UserIsAlreadyInvited(invitee.Id))
                        return false;

                    if (list.DoesContributorExist(invitee.Email))
                        return false;

                    if (inviteeAccountsListsDeclined != null)
                    {
                        inviteeAccountsListsDeclined.MakeInvited();
                    }
                    else if (inviteeAccountsListsLeft != null)
                    {
                        inviteeAccountsListsLeft.MakeInvited();
                    }
                    else
                    {
                        await _accountsListsRepository.AddAccountsListsInvitedAsync(invitee.Id, request.ListId);
                    }

                    await _todoListRepository.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }
    }
}