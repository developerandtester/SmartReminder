using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Infrastructure.Persistence;

namespace SmartReminder.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly SmartReminderDbContext _dbContext;

    public ChatRepository(SmartReminderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Conversation?> GetConversationByIdAsync(int conversationId)
    {
        return await _dbContext.Conversations
            .Include(x => x.Participants)
                .ThenInclude(x => x.User)
            .Include(x => x.Messages)
                .ThenInclude(x => x.SenderUser)
            .FirstOrDefaultAsync(x => x.Id == conversationId);
    }

    public async Task<List<Conversation>> GetConversationsForUserAsync(int userId)
    {
        return await _dbContext.Conversations
            .Include(x => x.Participants)
                .ThenInclude(x => x.User)
            .Include(x => x.Messages)
                .ThenInclude(x => x.SenderUser)
            .Where(x => x.Participants.Any(p => p.UserId == userId))
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task AddConversationAsync(Conversation conversation)
    {
        await _dbContext.Conversations.AddAsync(conversation);
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        await _dbContext.ChatMessages.AddAsync(message);
    }

    public async Task<List<ChatMessage>> GetMessagesAsync(int conversationId)
    {
        return await _dbContext.ChatMessages
            .Include(x => x.SenderUser)
            .Where(x => x.ConversationId == conversationId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}