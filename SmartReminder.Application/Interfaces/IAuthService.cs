using SmartReminder.Application.DTOs.Auth;

namespace SmartReminder.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);
}