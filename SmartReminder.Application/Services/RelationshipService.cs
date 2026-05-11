using SmartReminder.Application.DTOs.Relationships;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.Services;

public class RelationshipService : IRelationshipService
{
    private readonly IUserRepository _userRepository;
    private readonly IRelationshipRepository _relationshipRepository;

    public RelationshipService(
        IUserRepository userRepository,
        IRelationshipRepository relationshipRepository)
    {
        _userRepository = userRepository;
        _relationshipRepository = relationshipRepository;
    }

    public async Task<ParentLinkResponse> InviteStudentAsParentAsync(int currentUserId, InviteStudentRequest request)
    {
        var parent = await GetUserOrThrowAsync(currentUserId);

        if (parent.Role != UserRole.Parent)
        {
            throw new UnauthorizedAccessException("Only parents can invite students.");
        }

        var student = await GetStudentByEmailOrThrowAsync(request.StudentEmail);

        var existingLink = await _relationshipRepository.GetParentLinkAsync(student.Id, parent.Id);

        if (existingLink != null)
        {
            throw new InvalidOperationException("A parent-student link already exists.");
        }

        var link = new StudentParentLink
        {
            StudentId = student.Id,
            ParentId = parent.Id,
            Status = LinkStatus.Pending,
            CanViewProgress = true,
            CanCreateTasks = false,
            CanReceiveCriticalAlerts = false,
            CanViewVisualSchedule = true,
            CanViewMoodNotes = false
        };

        await _relationshipRepository.AddParentLinkAsync(link);
        await _relationshipRepository.SaveChangesAsync();

        var createdLink = await _relationshipRepository.GetParentLinkByIdAsync(link.Id);

        return MapParentLink(createdLink ?? link);
    }

    public async Task<TeacherLinkResponse> InviteStudentAsTeacherAsync(int currentUserId, InviteStudentRequest request)
    {
        var teacher = await GetUserOrThrowAsync(currentUserId);

        if (teacher.Role != UserRole.Teacher)
        {
            throw new UnauthorizedAccessException("Only teachers can invite students.");
        }

        var student = await GetStudentByEmailOrThrowAsync(request.StudentEmail);

        var existingLink = await _relationshipRepository.GetTeacherLinkAsync(student.Id, teacher.Id);

        if (existingLink != null)
        {
            throw new InvalidOperationException("A teacher-student link already exists.");
        }

        var link = new StudentTeacherLink
        {
            StudentId = student.Id,
            TeacherId = teacher.Id,
            Status = LinkStatus.Pending,
            CanViewProgress = true,
            CanCreateTasks = true,
            CanViewVisualSchedule = true
        };

        await _relationshipRepository.AddTeacherLinkAsync(link);
        await _relationshipRepository.SaveChangesAsync();

        var createdLink = await _relationshipRepository.GetTeacherLinkByIdAsync(link.Id);

        return MapTeacherLink(createdLink ?? link);
    }

    public async Task<PendingLinksResponse> GetPendingInvitesForStudentAsync(int currentUserId)
    {
        var student = await GetUserOrThrowAsync(currentUserId);

        if (student.Role != UserRole.Student)
        {
            throw new UnauthorizedAccessException("Only students can view pending invites.");
        }

        var parentLinks = await _relationshipRepository.GetParentLinksForStudentAsync(student.Id);
        var teacherLinks = await _relationshipRepository.GetTeacherLinksForStudentAsync(student.Id);

        return new PendingLinksResponse
        {
            ParentInvites = parentLinks
                .Where(x => x.Status == LinkStatus.Pending)
                .Select(MapParentLink)
                .ToList(),

            TeacherInvites = teacherLinks
                .Where(x => x.Status == LinkStatus.Pending)
                .Select(MapTeacherLink)
                .ToList()
        };
    }

    public async Task<List<ParentLinkResponse>> GetMyStudentsAsParentAsync(int currentUserId)
    {
        var parent = await GetUserOrThrowAsync(currentUserId);

        if (parent.Role != UserRole.Parent)
        {
            throw new UnauthorizedAccessException("Only parents can view linked students.");
        }

        var links = await _relationshipRepository.GetParentLinksForParentAsync(parent.Id);

        return links
            .Where(x => x.Status == LinkStatus.Accepted)
            .Select(MapParentLink)
            .ToList();
    }

    public async Task<List<TeacherLinkResponse>> GetMyStudentsAsTeacherAsync(int currentUserId)
    {
        var teacher = await GetUserOrThrowAsync(currentUserId);

        if (teacher.Role != UserRole.Teacher)
        {
            throw new UnauthorizedAccessException("Only teachers can view linked students.");
        }

        var links = await _relationshipRepository.GetTeacherLinksForTeacherAsync(teacher.Id);

        return links
            .Where(x => x.Status == LinkStatus.Accepted)
            .Select(MapTeacherLink)
            .ToList();
    }

    public async Task AcceptParentLinkAsync(int currentUserId, int linkId)
    {
        var link = await GetParentLinkOrThrowAsync(linkId);

        if (link.StudentId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the invited student can accept this parent link.");
        }

        link.Status = LinkStatus.Accepted;
        link.UpdatedAtUtc = DateTime.UtcNow;

        await _relationshipRepository.SaveChangesAsync();
    }

    public async Task RejectParentLinkAsync(int currentUserId, int linkId)
    {
        var link = await GetParentLinkOrThrowAsync(linkId);

        if (link.StudentId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the invited student can reject this parent link.");
        }

        link.Status = LinkStatus.Rejected;
        link.UpdatedAtUtc = DateTime.UtcNow;

        await _relationshipRepository.SaveChangesAsync();
    }

    public async Task AcceptTeacherLinkAsync(int currentUserId, int linkId)
    {
        var link = await GetTeacherLinkOrThrowAsync(linkId);

        if (link.StudentId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the invited student can accept this teacher link.");
        }

        link.Status = LinkStatus.Accepted;
        link.UpdatedAtUtc = DateTime.UtcNow;

        await _relationshipRepository.SaveChangesAsync();
    }

    public async Task RejectTeacherLinkAsync(int currentUserId, int linkId)
    {
        var link = await GetTeacherLinkOrThrowAsync(linkId);

        if (link.StudentId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the invited student can reject this teacher link.");
        }

        link.Status = LinkStatus.Rejected;
        link.UpdatedAtUtc = DateTime.UtcNow;

        await _relationshipRepository.SaveChangesAsync();
    }

    private async Task<AppUser> GetUserOrThrowAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return user;
    }

    private async Task<AppUser> GetStudentByEmailOrThrowAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Student email is required.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var student = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (student == null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        if (student.Role != UserRole.Student)
        {
            throw new InvalidOperationException("The invited user is not a student.");
        }

        return student;
    }

    private async Task<StudentParentLink> GetParentLinkOrThrowAsync(int linkId)
    {
        var link = await _relationshipRepository.GetParentLinkByIdAsync(linkId);

        if (link == null)
        {
            throw new KeyNotFoundException("Parent-student link not found.");
        }

        return link;
    }

    private async Task<StudentTeacherLink> GetTeacherLinkOrThrowAsync(int linkId)
    {
        var link = await _relationshipRepository.GetTeacherLinkByIdAsync(linkId);

        if (link == null)
        {
            throw new KeyNotFoundException("Teacher-student link not found.");
        }

        return link;
    }

    private static ParentLinkResponse MapParentLink(StudentParentLink link)
    {
        return new ParentLinkResponse
        {
            Id = link.Id,
            StudentId = link.StudentId,
            StudentName = link.Student?.FullName ?? string.Empty,
            StudentEmail = link.Student?.Email ?? string.Empty,
            ParentId = link.ParentId,
            ParentName = link.Parent?.FullName ?? string.Empty,
            ParentEmail = link.Parent?.Email ?? string.Empty,
            Status = link.Status,
            CanViewProgress = link.CanViewProgress,
            CanCreateTasks = link.CanCreateTasks,
            CanReceiveCriticalAlerts = link.CanReceiveCriticalAlerts,
            CanViewVisualSchedule = link.CanViewVisualSchedule,
            CanViewMoodNotes = link.CanViewMoodNotes
        };
    }

    private static TeacherLinkResponse MapTeacherLink(StudentTeacherLink link)
    {
        return new TeacherLinkResponse
        {
            Id = link.Id,
            StudentId = link.StudentId,
            StudentName = link.Student?.FullName ?? string.Empty,
            StudentEmail = link.Student?.Email ?? string.Empty,
            TeacherId = link.TeacherId,
            TeacherName = link.Teacher?.FullName ?? string.Empty,
            TeacherEmail = link.Teacher?.Email ?? string.Empty,
            Status = link.Status,
            CanViewProgress = link.CanViewProgress,
            CanCreateTasks = link.CanCreateTasks,
            CanViewVisualSchedule = link.CanViewVisualSchedule
        };
    }
}