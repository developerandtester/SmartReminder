using SmartReminder.Application.DTOs.Chat;

namespace SmartReminder.Application.Interfaces;

public interface IChatService
{
    Task<ConversationResponse> CreateConversationAsync(int currentUserId, CreateConversationRequest request);

    Task<List<ConversationResponse>> GetMyConversationsAsync(int currentUserId);

    Task<List<ChatMessageResponse>> GetMessagesAsync(int currentUserId, int conversationId);

    Task<ChatMessageResponse> SendMessageAsync(int currentUserId, SendMessageRequest request);
}