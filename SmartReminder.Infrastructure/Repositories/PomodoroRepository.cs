using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Infrastructure.Persistence;

namespace SmartReminder.Infrastructure.Repositories;

public class PomodoroRepository : IPomodoroRepository
{
    private readonly SmartReminderDbContext _dbContext;

    public PomodoroRepository(SmartReminderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PomodoroSession session)
    {
        await _dbContext.PomodoroSessions.AddAsync(session);
    }

    public async Task<PomodoroSession?> GetByIdAsync(int sessionId)
    {
        return await _dbContext.PomodoroSessions
            .Include(x => x.ReminderTask)
                .ThenInclude(x => x!.Steps)
            .FirstOrDefaultAsync(x => x.Id == sessionId);
    }

    public async Task<List<PomodoroSession>> GetSessionsForUserAsync(int userId)
    {
        return await _dbContext.PomodoroSessions
            .Include(x => x.ReminderTask)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.StartedAtUtc)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}