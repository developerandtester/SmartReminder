using SmartReminder.Application.DTOs.Chat;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;

    public ChatService(
        IChatRepository chatRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
    }

    public async Task<ConversationResponse> StartConversationAsync(
        int currentUserId,
        StartConversationRequest request)
    {
        if (request.OtherUserId <= 0)
        {
            throw new ArgumentException("Other user is required.");
        }

        if (request.OtherUserId == currentUserId)
        {
            throw new InvalidOperationException("You cannot start a conversation with yourself.");
        }

        var currentUser = await _userRepository.GetByIdAsync(currentUserId);

        if (currentUser == null)
        {
            throw new KeyNotFoundException("Current user not found.");
        }

        var otherUser = await _userRepository.GetByIdAsync(request.OtherUserId);

        if (otherUser == null)
        {
            throw new KeyNotFoundException("Other user not found.");
        }

        var existingConversation = await _chatRepository
            .GetConversationBetweenUsersAsync(currentUserId, request.OtherUserId);

        if (existingConversation != null)
        {
            return MapConversation(existingConversation);
        }

        var title = string.IsNullOrWhiteSpace(request.Title)
            ? $"{currentUser.FullName} and {otherUser.FullName}"
            : request.Title.Trim();

        var conversation = new Conversation
        {
            Type = ConversationType.UserToUser,
            Title = title,
            Participants = new List<ConversationParticipant>
            {
                new ConversationParticipant
                {
                    UserId = currentUserId
                },
                new ConversationParticipant
                {
                    UserId = request.OtherUserId
                }
            }
        };

        await _chatRepository.AddConversationAsync(conversation);
        await _chatRepository.SaveChangesAsync();

        var createdConversation = await _chatRepository.GetConversationByIdAsync(conversation.Id);

        return MapConversation(createdConversation ?? conversation);
    }

    public async Task<List<ConversationResponse>> GetMyConversationsAsync(int currentUserId)
    {
        var conversations = await _chatRepository.GetConversationsForUserAsync(currentUserId);

        return conversations
            .Select(MapConversation)
            .ToList();
    }

    public async Task<List<ChatMessageResponse>> GetMessagesAsync(int currentUserId, int conversationId)
    {
        var conversation = await GetConversationForUserOrThrowAsync(currentUserId, conversationId);

        var messages = await _chatRepository.GetMessagesAsync(conversation.Id);

        return messages
            .Select(MapMessage)
            .ToList();
    }

    public async Task<ChatMessageResponse> SendMessageAsync(
        int currentUserId,
        int conversationId,
        SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MessageText))
        {
            throw new ArgumentException("Message cannot be empty.");
        }

        var conversation = await GetConversationForUserOrThrowAsync(currentUserId, conversationId);

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

        return MapMessage(createdMessage);
    }

    public async Task<int> ConvertMessageToTaskAsync(
    int currentUserId,
    int messageId,
    ConvertMessageToTaskRequest request)
    {
        var message = await _chatRepository.GetMessageByIdAsync(messageId);

        if (message == null)
        {
            throw new KeyNotFoundException("Message not found.");
        }

        var isParticipant = message.Conversation.Participants
            .Any(x => x.UserId == currentUserId);

        if (!isParticipant)
        {
            throw new UnauthorizedAccessException("You are not part of this conversation.");
        }

        if (message.CreatedTaskId.HasValue)
        {
            throw new InvalidOperationException("Task already created from this message.");
        }

        var title = !string.IsNullOrWhiteSpace(request.Title)
            ? request.Title.Trim()
            : message.MessageText.Length > 60
                ? message.MessageText.Substring(0, 60)
                : message.MessageText;

        var task = new ReminderTask
        {
            OwnerUserId = currentUserId,
            CreatedByUserId = currentUserId,
            Title = title,
            Description = !string.IsNullOrWhiteSpace(request.Description)
        ? request.Description.Trim()
        : message.MessageText,
            DueAtUtc = request.DueDateUtc ?? DateTime.UtcNow.AddDays(1),
            Priority = request.HighPriority
        ? ReminderPriority.High
        : ReminderPriority.Medium,
            Status = ReminderStatus.Pending,
            SourceChatMessageId = message.Id
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        message.CreatedTaskId = task.Id;

        await _chatRepository.UpdateMessageAsync(message);
        await _chatRepository.SaveChangesAsync();

        return task.Id;
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

        var isParticipant = conversation.Participants
            .Any(x => x.UserId == currentUserId);

        if (!isParticipant)
        {
            throw new UnauthorizedAccessException("You are not part of this conversation.");
        }

        return conversation;
    }

    private static ConversationResponse MapConversation(Conversation conversation)
    {
        var lastMessage = conversation.Messages
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();

        return new ConversationResponse
        {
            Id = conversation.Id,
            Title = conversation.Title,
            ParticipantUserIds = conversation.Participants
                .Select(x => x.UserId)
                .ToList(),
            LastMessagePreview = lastMessage?.MessageText ?? string.Empty,
            CreatedAtUtc = conversation.CreatedAtUtc
        };
    }

    private static ChatMessageResponse MapMessage(ChatMessage message)
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