using System;
using System.Threading;
using System.Threading.Tasks;
using CodeJar.Gateway;
using MediatR;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories
{
    public class RedeemCouponUserStory : IRequestHandler<RedeemCoupon, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountDiscountRepository _accountDiscount;
        private readonly IDiscountRepository _discount;

        public RedeemCouponUserStory(IAccountRepository accountRepository, IAccountDiscountRepository accountDiscount, IDiscountRepository discount)
        {
            _accountRepository = accountRepository;
            _accountDiscount = accountDiscount;
            _discount = discount;
        }
        public async Task<bool> Handle(RedeemCoupon request, CancellationToken cancellationToken)
        {
            if (await _accountDiscount.GetUnredeemedDiscountByAccountIdAsync(request.AccountId) == null)
            {
                var gateway = new CodeJarGateway();

                var response = await gateway.RedeemCodeAsync(request.CouponCode);

                if (response.IsSuccess())
                {
                    var discountName = response.PromotionType;
                    var discount = await _discount.GetDiscountByNameAsync(discountName);

                    if (discount == null)
                        return false;

                    var accountDiscount = new AccountDiscount
                    {
                        Id = _accountDiscount.NextId(),
                        AccountId = request.AccountId,
                        DiscountId = discount.Id
                    };

                    await _accountDiscount.AddAsync(accountDiscount);
                    await _accountDiscount.SaveChangesAsync();

                    return true;
                }
            }

            return false;
        }
    }
}