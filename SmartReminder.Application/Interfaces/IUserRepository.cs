using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);

    Task<AppUser?> GetByEmailAsync(string email);

    Task AddAsync(AppUser user);

    Task SaveChangesAsync();
}
