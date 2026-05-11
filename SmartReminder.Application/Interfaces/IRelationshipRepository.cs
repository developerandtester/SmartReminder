using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IRelationshipRepository
{
    Task<StudentParentLink?> GetParentLinkAsync(int studentId, int parentId);
    Task<StudentTeacherLink?> GetTeacherLinkAsync(int studentId, int teacherId);

    Task<StudentParentLink?> GetParentLinkByIdAsync(int linkId);
    Task<StudentTeacherLink?> GetTeacherLinkByIdAsync(int linkId);

    Task<List<StudentParentLink>> GetParentLinksForStudentAsync(int studentId);
    Task<List<StudentTeacherLink>> GetTeacherLinksForStudentAsync(int studentId);

    Task<List<StudentParentLink>> GetParentLinksForParentAsync(int parentId);
    Task<List<StudentTeacherLink>> GetTeacherLinksForTeacherAsync(int teacherId);

    Task AddParentLinkAsync(StudentParentLink link);
    Task AddTeacherLinkAsync(StudentTeacherLink link);

    Task SaveChangesAsync();
}