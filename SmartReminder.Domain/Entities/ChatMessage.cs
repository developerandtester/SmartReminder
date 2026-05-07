using SmartReminder.Domain.Common;

namespace SmartReminder.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public int ConversationId { get; set; }

    public int SenderUserId { get; set; }

    public string MessageText { get; set; } = string.Empty;

    public bool IsFromLlm { get; set; }

    public bool CanBeConvertedToTask { get; set; }

    public int? CreatedTaskId { get; set; }

    public Conversation Conversation { get; set; } = null!;

    public AppUser SenderUser { get; set; } = null!;

    public ReminderTask? CreatedTask { get; set; }
}