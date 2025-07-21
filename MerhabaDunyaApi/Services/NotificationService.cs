using Microsoft.AspNetCore.SignalR;
using MerhabaDunyaApi.Hubs;

namespace MerhabaDunyaApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public Task SendNotificationAsync(int userId, string title, string message)
        {
            return _hub.Clients
                       .Group($"user_{userId}")
                       .SendAsync("ReceiveNotification", new { title, message, timestamp = DateTime.UtcNow });
        }
    }
}
