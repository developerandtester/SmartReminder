using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IChatRepository
{
    Task<Conversation?> GetConversationByIdAsync(int conversationId);

    Task<Conversation?> GetConversationBetweenUsersAsync(int userId, int otherUserId);

    Task<List<Conversation>> GetConversationsForUserAsync(int userId);

    Task<List<ChatMessage>> GetMessagesAsync(int conversationId);

    Task AddConversationAsync(Conversation conversation);

    Task AddMessageAsync(ChatMessage message);

    Task SaveChangesAsync();

    Task<ChatMessage?> GetMessageByIdAsync(int messageId);

    Task UpdateMessageAsync(ChatMessage message);
}