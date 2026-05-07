using Microsoft.EntityFrameworkCore;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Infrastructure.Persistence;

namespace SmartReminder.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SmartReminderDbContext _dbContext;

    public UserRepository(SmartReminderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(AppUser user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
