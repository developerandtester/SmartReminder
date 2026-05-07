using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(ReminderTask task);

    Task<List<ReminderTask>> GetTasksForUserAsync(int userId);

    Task<ReminderTask?> GetByIdAsync(int taskId);

    Task SaveChangesAsync();

    void Remove(ReminderTask task);
}