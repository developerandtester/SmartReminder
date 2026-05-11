using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IChatRepository
{
    Task<Conversation?> GetConversationByIdAsync(int conversationId);

    Task<List<Conversation>> GetConversationsForUserAsync(int userId);

    Task AddConversationAsync(Conversation conversation);

    Task AddMessageAsync(ChatMessage message);

    Task<List<ChatMessage>> GetMessagesAsync(int conversationId);

    Task SaveChangesAsync();
}