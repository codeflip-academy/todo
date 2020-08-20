using System;
using System.Threading;
using System.Threading.Tasks;
using CodeJar.Gateway;
using MediatR;
using Todo.Domain.Repositories;

namespace TodoWebAPI.UserStories
{
    public class RedeemCouponUserStory : IRequestHandler<RedeemCoupon, bool>
    {
        private readonly IAccountRepository _accountRepository;

        public RedeemCouponUserStory(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<bool> Handle(RedeemCoupon request, CancellationToken cancellationToken)
        {
            var gateway = new CodeJarGateway();

            var code = await gateway.RedeemCodeAsync(request.CouponCode);

            if (code.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}