namespace SmartReminder.Application.DTOs.Chat;

public class SendMessageRequest
{
    public int ConversationId { get; set; }

    public string MessageText { get; set; } = string.Empty;
}