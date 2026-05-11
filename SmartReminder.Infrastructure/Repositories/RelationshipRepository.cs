using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Infrastructure.Persistence;

namespace SmartReminder.Infrastructure.Repositories;

public class RelationshipRepository : IRelationshipRepository
{
    private readonly SmartReminderDbContext _dbContext;

    public RelationshipRepository(SmartReminderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentParentLink?> GetParentLinkAsync(int studentId, int parentId)
    {
        return await _dbContext.StudentParentLinks
            .Include(x => x.Student)
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ParentId == parentId);
    }

    public async Task<StudentTeacherLink?> GetTeacherLinkAsync(int studentId, int teacherId)
    {
        return await _dbContext.StudentTeacherLinks
            .Include(x => x.Student)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.TeacherId == teacherId);
    }

    public async Task<StudentParentLink?> GetParentLinkByIdAsync(int linkId)
    {
        return await _dbContext.StudentParentLinks
            .Include(x => x.Student)
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == linkId);
    }

    public async Task<StudentTeacherLink?> GetTeacherLinkByIdAsync(int linkId)
    {
        return await _dbContext.StudentTeacherLinks
            .Include(x => x.Student)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == linkId);
    }

    public async Task<List<StudentParentLink>> GetParentLinksForStudentAsync(int studentId)
    {
        return await _dbContext.StudentParentLinks
            .Include(x => x.Student)
            .Include(x => x.Parent)
            .Where(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<List<StudentTeacherLink>> GetTeacherLinksForStudentAsync(int studentId)
    {
        return await _dbContext.StudentTeacherLinks
            .Include(x => x.Student)
            .Include(x => x.Teacher)
            .Where(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<List<StudentParentLink>> GetParentLinksForParentAsync(int parentId)
    {
        return await _dbContext.StudentParentLinks
            .Include(x => x.Student)
            .Include(x => x.Parent)
            .Where(x => x.ParentId == parentId)
            .ToListAsync();
    }

    public async Task<List<StudentTeacherLink>> GetTeacherLinksForTeacherAsync(int teacherId)
    {
        return await _dbContext.StudentTeacherLinks
            .Include(x => x.Student)
            .Include(x => x.Teacher)
            .Where(x => x.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task AddParentLinkAsync(StudentParentLink link)
    {
        await _dbContext.StudentParentLinks.AddAsync(link);
    }

    public async Task AddTeacherLinkAsync(StudentTeacherLink link)
    {
        await _dbContext.StudentTeacherLinks.AddAsync(link);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}