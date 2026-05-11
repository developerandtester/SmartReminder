namespace SmartReminder.Application.DTOs.Chat;

public class ChatMessageResponse
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderUserId { get; set; }

    public string SenderName { get; set; } = string.Empty;

    public string MessageText { get; set; } = string.Empty;

    public bool IsFromLlm { get; set; }

    public bool CanBeConvertedToTask { get; set; }

    public int? CreatedTaskId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}