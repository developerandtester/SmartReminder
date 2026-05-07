using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Infrastructure.Persistence;

namespace SmartReminder.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly SmartReminderDbContext _dbContext;

    public TaskRepository(SmartReminderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ReminderTask task)
    {
        await _dbContext.ReminderTasks.AddAsync(task);
    }

    public async Task<List<ReminderTask>> GetTasksForUserAsync(int userId)
    {
        return await _dbContext.ReminderTasks
            .Include(x => x.Steps)
            .Where(x => x.OwnerUserId == userId)
            .OrderBy(x => x.DueAtUtc)
            .ToListAsync();
    }

    public async Task<ReminderTask?> GetByIdAsync(int taskId)
    {
        return await _dbContext.ReminderTasks
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == taskId);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Remove(ReminderTask task)
    {
        _dbContext.ReminderTasks.Remove(task);
    }
}