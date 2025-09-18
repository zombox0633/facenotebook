using Microsoft.EntityFrameworkCore;
using user.data;
using user.dto;
using user.iservice;
using user.model;
using helper.ihashPassword;
using helper.ijwt;

namespace user.service;

public class UserService : IUserService
{
  private readonly ApplicationDbContext _context;
  private readonly IHashPassword _hashPassword;
  private readonly IJwtService _jwtService;

  public UserService(ApplicationDbContext context, IHashPassword hashPassword, IJwtService jwt)
  {
    _context = context;
    _hashPassword = hashPassword;
    _jwtService = jwt;
  }

  //--------------------------- Authentication ----------------------------
  public async Task<TokenResponse?> LoginAsync(LoginRequest loginRequest)
  {
    var user = await _context.Users
      .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

    if (user == null)
      return null;

    var IsValidPassword = await _hashPassword.VerifyPasswordAsync(loginRequest.Password, user.Password);
    if (!IsValidPassword)
      return null;

    var accessToken = _jwtService.GenerateAccessToken(user);
    var refreshToken = _jwtService.GenerateRefreshToken();
    var expiryTime = _jwtService.GetRefreshTokenExpiry();

    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiryTime = DateTime.SpecifyKind(expiryTime, DateTimeKind.Unspecified);
    user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    await _context.SaveChangesAsync();

    return new TokenResponse
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken
    };
  }

  public async Task<bool> LogoutAsync(string refreshToken)
  {
    var user = await _context.Users
      .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    if (user == null)
      return false;

    user.RefreshToken = null;
    user.RefreshTokenExpiryTime = null;
    user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
  {
    var user = await _context.Users
      .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    if (user == null || user.RefreshTokenExpiryTime <= DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified))
      return null;

    var newAccessToken = _jwtService.GenerateAccessToken(user);
    var newRefreshToken = _jwtService.GenerateRefreshToken();
    var expiryTime = _jwtService.GetRefreshTokenExpiry();

    user.RefreshToken = newRefreshToken;
    user.RefreshTokenExpiryTime = DateTime.SpecifyKind(expiryTime, DateTimeKind.Unspecified);
    user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    await _context.SaveChangesAsync();

    return new TokenResponse
    {
      AccessToken = newAccessToken,
      RefreshToken = newRefreshToken
    };
  }



  //--------------------------- User ----------------------------------
  public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
  {
    var users = await _context.Users
      .OrderBy(u => u.CreatedAt)
      .ToListAsync();

    return users.Select(MapToResponse);
  }

  public async Task<UserResponse?> GetCurrentUserAsync(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    return user == null ? null : MapToResponse(user);
  }

  public async Task<UserResponse> CreateUserAsync(CreateUserRequest createUserDto)
  {
    if (await EmailExistsAsync(createUserDto.Email))
    {
      throw new InvalidOperationException("Email already exists");
    }
    if (!_hashPassword.IsValidPassword(createUserDto.Password))
    {
      throw new InvalidOperationException("Password must contain at least 8 characters with uppercase, lowercase, and numbers");
    }

    var hashedPassword = await _hashPassword.HashPasswordAsync(createUserDto.Password);
    var user = new User
    {
      Id = Guid.NewGuid(),
      Name = createUserDto.Name,
      Email = createUserDto.Email,
      Password = hashedPassword,
      RefreshToken = null,
      RefreshTokenExpiryTime = null,
      CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
      UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return MapToResponse(user);
  }

  public async Task<bool> DeleteUserAsync(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
      return false;
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return true;
  }

  //--------------------------- Helper ------------------------------

  private static UserResponse MapToResponse(User user)
  {
    return new UserResponse
    {
      Id = user.Id,
      Name = user.Name,
      Email = user.Email,
      CreatedAt = user.CreatedAt,
      UpdatedAt = user.UpdatedAt
    };
  }

  public async Task<bool> UserExistsAsync(Guid id)
  {
    return await _context.Users.AnyAsync(u => u.Id == id);
  }

  public async Task<bool> EmailExistsAsync(string email)
  {
    return await _context.Users.AnyAsync(u => u.Email == email);
  }
}