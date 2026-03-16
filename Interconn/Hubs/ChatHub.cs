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
        private static readonly ConcurrentDictionary<string, string> _onlineUsers= new ConcurrentDictionary<string, string>();

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        //使用者呼叫connection.start(), SignalR 偵測到新連線, 就會呼叫這個方法
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;

            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //取得使用者的資料是null的話 斷開連線
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userIdStr))
            {
                Context.Abort();
                return;
            }

            //這裡把使用者加入線上清單
            _onlineUsers[Context.ConnectionId] = userName;

            // 廣播在線名單給所有人
            await Clients.All.SendAsync("UserList", _onlineUsers.Values.Distinct());

            // 載入歷史訊息給自己
            var messages = await _chatService.GetRecentMessagesAsync();
            await Clients.Caller.SendAsync("LoadHistoryResult", messages);

            await base.OnConnectedAsync();
        }

        //每當使用者斷開連線時, SignalR會自動呼叫這個方法
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //把使用者從線上清單移除
            _onlineUsers.TryRemove(Context.ConnectionId, out _);

            await Clients.All.SendAsync("UserList", _onlineUsers.Values.Distinct());

            await base.OnDisconnectedAsync(exception);
        }

        //前端JavaScript呼叫 Connection.invoke("SendMessage", message),就會進入這個方法
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