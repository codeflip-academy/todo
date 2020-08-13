using Braintree;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.BraintreeService;
using TodoWebAPI.Models;

namespace TodoWebAPI.UserStories.CreateAccount
{
    public class CreateAccountUserStory : IRequestHandler<CreateAccountModel, Account>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IBraintreeConfiguration _braintreeConfiguration;

        public CreateAccountUserStory(IAccountRepository accountRepository, IBraintreeConfiguration braintreeConfiguration)
        {
            _accountRepository = accountRepository;
            _braintreeConfiguration = braintreeConfiguration;
        }
        public async Task<Account> Handle(CreateAccountModel request, CancellationToken cancellationToken)
        {
            var gateway = _braintreeConfiguration.GetGateway();

            var account = await _accountRepository.FindAccountByEmailAsync(request.Email);

            if (account != null)
            {
                return account;
            }
            
            var fullName = request.FullName.Split(' ', 2);

            var paymentRequest = new CustomerRequest
            {
                FirstName = fullName[0],
                LastName = fullName[1],
                Email = request.Email
            };

            Result<Customer> customerResult = await gateway.Customer.CreateAsync(paymentRequest);

            bool success = customerResult.IsSuccess();

            var customerId = customerResult.Target.Id;

            account = new Account(_accountRepository.NextId(), request.Email, PlanTiers.Free)
            {
                FullName = request.FullName,
                PictureUrl = request.PictureUrl,
                PaymentId = customerId
            };

            _accountRepository.AddAccount(account);

            await _accountRepository.SaveChangesAsync();

            return account;
        }
    }
}
