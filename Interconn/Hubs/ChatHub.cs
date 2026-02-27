using Interconn.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Interconn.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        // 管理在線使用者 (ConnectionId -> UserName)
        private static readonly ConcurrentDictionary<string, string> _onlineUsers
            = new();

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;

            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userIdStr))
            {
                Context.Abort();
                return;
            }

            _onlineUsers[Context.ConnectionId] = userName;

            // 廣播在線名單給所有人
            await Clients.All.SendAsync("UserList", _onlineUsers.Values.Distinct());

            // 載入歷史訊息給自己
            var messages = await _chatService.GetRecentMessagesAsync();
            await Clients.Caller.SendAsync("LoadHistoryResult", messages);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _onlineUsers.TryRemove(Context.ConnectionId, out _);

            await Clients.All.SendAsync("UserList", _onlineUsers.Values.Distinct());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            var userName = Context.User?.Identity?.Name;
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 加上 log 方便排查
            Console.WriteLine($"Hub 收到訊息, UserName={userName}, UserId={userIdStr}, Message={message}");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userIdStr) || string.IsNullOrWhiteSpace(message))
                return;

            int userId = int.Parse(userIdStr);

            // 存到資料庫前也可以加 log
            Console.WriteLine($"存訊息到 DB: {userName} => {message}");

            // 存到資料庫
            await _chatService.SaveMessageAsync(userId, userName, message);

            // 廣播給所有人
            await Clients.All.SendAsync("ReceiveMessage",
                userName,
                message,
                DateTime.Now.ToString("HH:mm"));
        }
    }
}