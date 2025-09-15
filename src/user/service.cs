using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using user.data;
using user.dto;
using user.iservice;
using user.model;

namespace user.service;

public class UserService : IUserService
{
  private readonly ApplicationDbContext _context;

  public UserService(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
  {
    var users = await _context.Users
    .OrderBy(u => u.CreatedAt)
    .ToListAsync();

    return users.Select(MapToResponseDto);
  }

  public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    return user == null ? null : MapToResponseDto(user);
  }

  public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
  {
    if (await EmailExistsAsync(createUserDto.Email))
    {
      throw new InvalidCastException("Email already exists");
    }

    var hashedPassword = await HashPasswordAsync(createUserDto.Password);
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

  //--------------------------- Util ------------------------------

  private static UserResponseDto MapToResponseDto(User user)
  {
    return new UserResponseDto
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

  //-------------------------------- HashPassword -----------------------------------
  private static async Task<string> HashPasswordAsync(string password)
  {
    var salt = new byte[32]; // 32 bytes
    using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    rng.GetBytes(salt);

    var passwordBytes = Encoding.UTF8.GetBytes(password);
    using var argon2 = new Argon2id(passwordBytes)
    {
      Salt = salt,
      DegreeOfParallelism = 8, // จำนวน threads
      MemorySize = 65536, // 64 MB
      Iterations = 4 // จำนวนรอบ
    };

    // Generate hash
    var hash = await argon2.GetBytesAsync(32); // 32 bytes
    var combined = new byte[salt.Length + hash.Length]; // 32 + 32 = 64 bytes
    Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
    Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

    // bytes → string
    return Convert.ToBase64String(combined); // ~88 characters
  }

  private static async Task<bool> VerifyPasswordInternalAsync(string password, string hashedPassword)
  {
    try
    {
      // string → bytes
      var combined = Convert.FromBase64String(hashedPassword);

      var salt = new byte[32];
      var hash = new byte[32];
      Buffer.BlockCopy(combined, 0, salt, 0, 32);
      Buffer.BlockCopy(combined, 32, hash, 0, 32);

      var passwordBytes = Encoding.UTF8.GetBytes(password);
      using var argon2 = new Argon2id(passwordBytes)
      {
        Salt = salt,
        DegreeOfParallelism = 8,
        MemorySize = 65536,
        Iterations = 4
      };

      var newHash = await argon2.GetBytesAsync(32);
      return hash.SequenceEqual(newHash);
    }
    catch
    {
      return false;
    }
  }
}
