using Interconn.Models;
using Microsoft.EntityFrameworkCore;

namespace Interconn.Services
{
    public class ChatService
    {
        private readonly InterconnDbContext _context;

        public ChatService(InterconnDbContext context)
        {
            _context = context;
        }

        //儲存聊天訊息的方法
        public async Task SaveMessageAsync(int userId, string userName, string message)
        {
            var chat = new ChatMessage
            {
                UserId = userId,
                UserName = userName,
                Message = message,
                CreatedTime = DateTime.Now
            };

            _context.ChatMessages.Add(chat);
            await _context.SaveChangesAsync();
           
        }

        //去資料庫撈最近的聊天訊息,預設撈50筆,並且按照時間排序
        //OrderByDescending先取最新的50筆(倒序),取出前count筆,再OrderBy改成正序
        public async Task<List<ChatMessage>> GetRecentMessagesAsync(int count = 50)
        {
            IQueryable<ChatMessage> recentMessage =  _context.ChatMessages
                                                    .OrderByDescending(x => x.CreatedTime)
                                                    .Take(count)
                                                    .OrderBy(x => x.CreatedTime);
                
            List<ChatMessage> result = await recentMessage.ToListAsync();

            return result;
        }
    }
}
