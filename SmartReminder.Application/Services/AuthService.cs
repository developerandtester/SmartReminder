using SmartReminder.Application.DTOs.Auth;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        ValidateRegisterRequest(request);

        var email = NormalizeEmail(request.Email);

        var emailExists = await _userRepository.EmailExistsAsync(email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        ValidateLoginRequest(request);

        var email = NormalizeEmail(request.Email);

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive.");
        }

        var passwordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(AppUser user)
    {
        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = _jwtTokenService.GenerateToken(user)
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static void ValidateRegisterRequest(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }

        if (request.Password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters.");
        }

        if (!Enum.IsDefined(typeof(UserRole), request.Role))
        {
            throw new ArgumentException("Invalid user role.");
        }
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }
    }
}