using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.DTOs.Tasks;
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

    public async Task<List<ReminderTask>> GetFilteredTasksForUserAsync(int userId, TaskFilterRequest filter)
    {
        var query = _dbContext.ReminderTasks
            .Include(x => x.Steps)
            .Where(x => x.OwnerUserId == userId)
            .AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(x => x.Priority == filter.Priority.Value);
        }

        if (filter.DueFromUtc.HasValue)
        {
            query = query.Where(x => x.DueAtUtc >= filter.DueFromUtc.Value);
        }

        if (filter.DueToUtc.HasValue)
        {
            query = query.Where(x => x.DueAtUtc <= filter.DueToUtc.Value);
        }

        return await query
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