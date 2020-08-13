using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationWhenSubItemMovedToTrashEventHandler : INotificationHandler<SubItemMovedToTrash>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListItemRepository _todoListItemRepository;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationWhenSubItemMovedToTrashEventHandler(IHubContext<NotificationHub> hubContext, ITodoListItemRepository todoListItem, ITodoListRepository todoList, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _todoListItemRepository = todoListItem;
            _todoListRepository = todoList;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(SubItemMovedToTrash notification, CancellationToken cancellationToken)
        {
            var item = await _todoListItemRepository.FindToDoListItemByIdAsync(notification.ItemId.GetValueOrDefault());
            var list = await _todoListRepository.FindTodoListIdByIdAsync(item.ListId.GetValueOrDefault());

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(list, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hubContext.Clients.Users(contributorExceptYou).SendAsync("SubItemTrashed", notification.ItemId, notification.SubItem);
        }
    }
}
