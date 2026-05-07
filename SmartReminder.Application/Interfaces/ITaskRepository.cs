using SmartReminder.Application.DTOs.Tasks;
using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(ReminderTask task);

    Task<List<ReminderTask>> GetTasksForUserAsync(int userId);

    Task<List<ReminderTask>> GetFilteredTasksForUserAsync(int userId, TaskFilterRequest filter);

    Task<ReminderTask?> GetByIdAsync(int taskId);

    Task SaveChangesAsync();

    void Remove(ReminderTask task);
}