using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace MerhabaDunyaApi.Hubs
{
    public class NotificationHub : Hub
    {
        public Task Subscribe(int userId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}
