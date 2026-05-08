using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IVisualScheduleRepository
{
    Task AddAsync(VisualSchedule schedule);

    Task<VisualSchedule?> GetByIdAsync(int scheduleId);

    Task<VisualSchedule?> GetForStudentByDateAsync(int studentId, DateOnly date);

    Task SaveChangesAsync();

    void Remove(VisualSchedule schedule);
}