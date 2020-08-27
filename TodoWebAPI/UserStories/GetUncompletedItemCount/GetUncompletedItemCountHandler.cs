using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace TodoWebAPI.UserStories
{
    public class GetUncompletedItemCountHandler : IRequestHandler<GetUncompletedItemCount, int>
    {
        private readonly DapperQuery _dapper;

        public GetUncompletedItemCountHandler(DapperQuery dapper)
        {
            _dapper = dapper;
        }

        public async Task<int> Handle(GetUncompletedItemCount request, CancellationToken cancellationToken)
        {
            var uncompletedItems = await _dapper.GetUncompletedItemsByListIdAsync(request.ListId);

            return uncompletedItems.Count;
        }
    }
}