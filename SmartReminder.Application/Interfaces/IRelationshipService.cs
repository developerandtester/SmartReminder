using SmartReminder.Application.DTOs.Relationships;

namespace SmartReminder.Application.Interfaces;

public interface IRelationshipService
{
    Task<ParentLinkResponse> InviteStudentAsParentAsync(int currentUserId, InviteStudentRequest request);

    Task<TeacherLinkResponse> InviteStudentAsTeacherAsync(int currentUserId, InviteStudentRequest request);

    Task<PendingLinksResponse> GetPendingInvitesForStudentAsync(int currentUserId);

    Task<List<ParentLinkResponse>> GetMyStudentsAsParentAsync(int currentUserId);

    Task<List<TeacherLinkResponse>> GetMyStudentsAsTeacherAsync(int currentUserId);

    Task AcceptParentLinkAsync(int currentUserId, int linkId);

    Task RejectParentLinkAsync(int currentUserId, int linkId);

    Task AcceptTeacherLinkAsync(int currentUserId, int linkId);

    Task RejectTeacherLinkAsync(int currentUserId, int linkId);
}