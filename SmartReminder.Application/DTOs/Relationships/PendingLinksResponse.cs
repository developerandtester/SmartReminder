namespace SmartReminder.Application.DTOs.Relationships;

public class PendingLinksResponse
{
    public List<ParentLinkResponse> ParentInvites { get; set; } = new();

    public List<TeacherLinkResponse> TeacherInvites { get; set; } = new();
}