using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;
using Todo.Infrastructure.PaymentMethods;
using TodoWebAPI.BraintreeService;
using TodoWebAPI.UserStories.RoleChanges;

namespace TodoWebAPI.UserStories
{
    public class ChangeSubscriptionUserStory : IRequestHandler<ChangeSubscription, bool>
    {
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly IPaymentMethodRepository _paymentMethod;
        private readonly IAccountRepository _account;
        private readonly IAccountDiscountRepository _accountDiscountRepository;
        private readonly IDiscountRepository _discountRepository;

        public ChangeSubscriptionUserStory(IBraintreeConfiguration braintreeConfiguration,
            IPaymentMethodRepository paymentMethod,
            IAccountRepository account,
            IAccountDiscountRepository accountDiscountRepository,
            IDiscountRepository discountRepository)
        {
            _braintreeConfiguration = braintreeConfiguration;
            _paymentMethod = paymentMethod;
            _account = account;
            _accountDiscountRepository = accountDiscountRepository;
            _discountRepository = discountRepository;
        }

        public async Task<bool> Handle(ChangeSubscription request, CancellationToken cancellationToken)
        {
            var account = await _account.FindAccountByIdAsync(request.AccountId);

            if (account.HasPaymentMethod())
            {
                var gateway = _braintreeConfiguration.GetGateway();
                var plans = await gateway.Plan.AllAsync();
                var plan = (from p in plans where p.Name == request.Plan select p).FirstOrDefault();
                var planId = SubscriptionHelper.ConvertPlanToBrainTreeType(request.Plan);
                var accountDiscount = await _accountDiscountRepository.GetUnredeemedDiscountByAccountIdAsync(request.AccountId);

                var updateSubscriptionRequest = new SubscriptionRequest
                {
                    PaymentMethodToken = account.PaymentMethodId,
                    PlanId = planId,
                    Price = plan.Price,
                };

                if (accountDiscount != null)
                {
                    var discount = await _discountRepository.GetDiscountByIdAsync(accountDiscount.DiscountId);

                    updateSubscriptionRequest.Discounts = new DiscountsRequest
                    {
                        Add = new AddDiscountRequest[]
                            {
                                new AddDiscountRequest
                                {
                                    InheritedFromId = discount.Id.ToString(),
                                    Amount = DiscountCalculator.CalculateDiscount(
                                        price: plan.Price.GetValueOrDefault(),
                                        percentage: discount.Percentage)
                                }
                            },
                    };
                }

                var updateSubscriptionResult = await gateway.Subscription.UpdateAsync(account.SubscriptionId, updateSubscriptionRequest);

                if (updateSubscriptionResult.IsSuccess())
                {
                    accountDiscount.ApplyDiscountToSubscription();

                    await _accountDiscountRepository.SaveChangesAsync();

                    return true;
                }
            }

            return false;
        }
    }
}