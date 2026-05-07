using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IPomodoroRepository
{
    Task AddAsync(PomodoroSession session);

    Task<PomodoroSession?> GetByIdAsync(int sessionId);

    Task<List<PomodoroSession>> GetSessionsForUserAsync(int userId);

    Task SaveChangesAsync();
}