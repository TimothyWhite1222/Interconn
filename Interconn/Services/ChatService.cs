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

        public async Task<List<ChatMessage>> GetRecentMessagesAsync(int count = 50)
        {
            return await _context.ChatMessages
                .OrderByDescending(x => x.CreatedTime)
                .Take(count)
                .OrderBy(x => x.CreatedTime)
                .ToListAsync();
        }
    }
}
