namespace SmartReminder.Application.DTOs.Chat;

public class StartConversationRequest
{
    public int OtherUserId { get; set; }

    public string Title { get; set; } = string.Empty;
}