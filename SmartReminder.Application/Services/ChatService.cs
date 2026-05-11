using SmartReminder.Application.DTOs.Chat;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;

    public ChatService(
        IChatRepository chatRepository,
        IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<ConversationResponse> CreateConversationAsync(
        int currentUserId,
        CreateConversationRequest request)
    {
        if (request.ParticipantUserId <= 0)
        {
            throw new ArgumentException("Participant user id is required.");
        }

        if (request.ParticipantUserId == currentUserId)
        {
            throw new InvalidOperationException("You cannot start a conversation with yourself.");
        }

        var currentUser = await _userRepository.GetByIdAsync(currentUserId);

        if (currentUser == null)
        {
            throw new KeyNotFoundException("Current user not found.");
        }

        var participant = await _userRepository.GetByIdAsync(request.ParticipantUserId);

        if (participant == null)
        {
            throw new KeyNotFoundException("Participant user not found.");
        }

        var title = string.IsNullOrWhiteSpace(request.Title)
            ? $"{currentUser.FullName} and {participant.FullName}"
            : request.Title.Trim();

        var conversation = new Conversation
        {
            Title = title,
            Type = ConversationType.UserToUser,
            Participants = new List<ConversationParticipant>
            {
                new ConversationParticipant
                {
                    UserId = currentUserId
                },
                new ConversationParticipant
                {
                    UserId = request.ParticipantUserId
                }
            }
        };

        await _chatRepository.AddConversationAsync(conversation);
        await _chatRepository.SaveChangesAsync();

        var created = await _chatRepository.GetConversationByIdAsync(conversation.Id);

        return MapConversationToResponse(created ?? conversation);
    }

    public async Task<List<ConversationResponse>> GetMyConversationsAsync(int currentUserId)
    {
        var conversations = await _chatRepository.GetConversationsForUserAsync(currentUserId);

        return conversations
            .Select(MapConversationToResponse)
            .ToList();
    }

    public async Task<List<ChatMessageResponse>> GetMessagesAsync(int currentUserId, int conversationId)
    {
        var conversation = await GetConversationForUserOrThrowAsync(currentUserId, conversationId);

        var messages = await _chatRepository.GetMessagesAsync(conversation.Id);

        return messages
            .OrderBy(x => x.CreatedAtUtc)
            .Select(MapMessageToResponse)
            .ToList();
    }

    public async Task<ChatMessageResponse> SendMessageAsync(int currentUserId, SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MessageText))
        {
            throw new ArgumentException("Message text is required.");
        }

        var conversation = await GetConversationForUserOrThrowAsync(currentUserId, request.ConversationId);

        var message = new ChatMessage
        {
            ConversationId = conversation.Id,
            SenderUserId = currentUserId,
            MessageText = request.MessageText.Trim(),
            IsFromLlm = false,
            CanBeConvertedToTask = true
        };

        await _chatRepository.AddMessageAsync(message);
        await _chatRepository.SaveChangesAsync();

        var messages = await _chatRepository.GetMessagesAsync(conversation.Id);
        var createdMessage = messages.First(x => x.Id == message.Id);

        return MapMessageToResponse(createdMessage);
    }

    private async Task<Conversation> GetConversationForUserOrThrowAsync(
        int currentUserId,
        int conversationId)
    {
        var conversation = await _chatRepository.GetConversationByIdAsync(conversationId);

        if (conversation == null)
        {
            throw new KeyNotFoundException("Conversation not found.");
        }

        var isParticipant = conversation.Participants.Any(x => x.UserId == currentUserId);

        if (!isParticipant)
        {
            throw new UnauthorizedAccessException("You are not part of this conversation.");
        }

        return conversation;
    }

    private static ConversationResponse MapConversationToResponse(Conversation conversation)
    {
        var lastMessage = conversation.Messages
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();

        return new ConversationResponse
        {
            Id = conversation.Id,
            Title = conversation.Title,
            Type = conversation.Type.ToString(),
            ParticipantUserIds = conversation.Participants
                .Select(x => x.UserId)
                .ToList(),
            LastMessage = lastMessage?.MessageText ?? string.Empty,
            CreatedAtUtc = conversation.CreatedAtUtc
        };
    }

    private static ChatMessageResponse MapMessageToResponse(ChatMessage message)
    {
        return new ChatMessageResponse
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            SenderName = message.SenderUser?.FullName ?? string.Empty,
            MessageText = message.MessageText,
            IsFromLlm = message.IsFromLlm,
            CanBeConvertedToTask = message.CanBeConvertedToTask,
            CreatedTaskId = message.CreatedTaskId,
            CreatedAtUtc = message.CreatedAtUtc
        };
    }
}