using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Todo.Domain.DomainEvents;
using Todo.Domain.Repositories;
using Todo.Infrastructure;
using TodoWebAPI.SignalR;

namespace TodoWebAPI.DomainEventHandlers
{
    public class SendNotificationToClientWhenListIsDeleted
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITodoListRepository _todoListRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendNotificationToClientWhenListIsDeleted(IHubContext<NotificationHub> hubContext, ITodoListRepository todoListRepository, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _todoListRepository = todoListRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Handle(TrashedList notification, CancellationToken cancellationToken)
        {
            var todoList = await _todoListRepository.FindTodoListIdByIdAsync(notification.ListId);

            var contributorExceptYou = RemoveSelfFromContributorSignalRHelper.RemoveContributor(todoList, _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            await _hubContext.Clients.Users(contributorExceptYou).SendAsync("ListTrashed", notification.ListId);
        }
    }
}