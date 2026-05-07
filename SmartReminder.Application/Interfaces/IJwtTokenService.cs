using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(AppUser user);
}