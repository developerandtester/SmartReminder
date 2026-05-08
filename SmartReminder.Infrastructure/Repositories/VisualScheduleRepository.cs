using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Infrastructure.Persistence;

namespace SmartReminder.Infrastructure.Repositories;

public class VisualScheduleRepository : IVisualScheduleRepository
{
    private readonly SmartReminderDbContext _dbContext;

    public VisualScheduleRepository(SmartReminderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(VisualSchedule schedule)
    {
        await _dbContext.VisualSchedules.AddAsync(schedule);
    }

    public async Task<VisualSchedule?> GetByIdAsync(int scheduleId)
    {
        return await _dbContext.VisualSchedules
            .Include(x => x.Items)
                .ThenInclude(x => x.LinkedTask)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
    }

    public async Task<VisualSchedule?> GetForStudentByDateAsync(int studentId, DateOnly date)
    {
        return await _dbContext.VisualSchedules
            .Include(x => x.Items)
                .ThenInclude(x => x.LinkedTask)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ScheduleDate == date);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Remove(VisualSchedule schedule)
    {
        _dbContext.VisualSchedules.Remove(schedule);
    }
}