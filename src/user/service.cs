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



  //--------------------------------------------------------------

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

}
