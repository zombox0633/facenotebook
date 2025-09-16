using Microsoft.EntityFrameworkCore;
using user.data;
using user.dto;
using user.iservice;
using user.model;
using utils.ihashPassword;

namespace user.service;

public class UserService : IUserService
{
  private readonly ApplicationDbContext _context;
  private readonly IHashPassword _hashPassword;

  public UserService(ApplicationDbContext context, IHashPassword hashPassword)
  {
    _context = context;
    _hashPassword = hashPassword;
  }
  
  //------------------------- User ----------------------------------
  public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
  {
    var users = await _context.Users
    .OrderBy(u => u.CreatedAt)
    .ToListAsync();

    return users.Select(MapToResponseDto);
  }

  public async Task<UserResponse?> GetUserByIdAsync(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    return user == null ? null : MapToResponseDto(user);
  }

  public async Task<UserResponse> CreateUserAsync(CreateUserRequest createUserDto)
  {
    if (await EmailExistsAsync(createUserDto.Email))
    {
      throw new InvalidCastException("Email already exists");
    }
    if (!_hashPassword.IsValidPassword(createUserDto.Password))
    {
      throw new ArgumentException("Password must contain at least 8 characters with uppercase, lowercase, and numbers");
    }

    var hashedPassword = await _hashPassword.HashPasswordAsync(createUserDto.Password);
    var user = new User
    {
      Id = Guid.NewGuid(),
      Name = createUserDto.Name,
      Email = createUserDto.Email,
      Password = hashedPassword,
      CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
      UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return MapToResponseDto(user);
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

  private static UserResponse MapToResponseDto(User user)
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