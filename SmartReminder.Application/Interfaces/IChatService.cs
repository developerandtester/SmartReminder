using SmartReminder.Application.DTOs.Chat;

namespace SmartReminder.Application.Interfaces;

public interface IChatService
{
    Task<ConversationResponse> StartConversationAsync(int currentUserId, StartConversationRequest request);

    Task<List<ConversationResponse>> GetMyConversationsAsync(int currentUserId);

    Task<List<ChatMessageResponse>> GetMessagesAsync(int currentUserId, int conversationId);

    Task<ChatMessageResponse> SendMessageAsync(int currentUserId, int conversationId, SendMessageRequest request);

    Task<int> ConvertMessageToTaskAsync(int currentUserId,int messageId, ConvertMessageToTaskRequest request);
}