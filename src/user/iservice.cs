

using user.dto;

namespace user.iservice;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    // Task<UserResponseDto?> GetUserByEmailAsync(string email);
    // Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
    // Task<UserResponseDto?> UpdateNameAsync(Guid id, UpdateNameDto updateUserDto);
    // Task<bool> DeleteUserAsync(Guid id);
    // Task<bool> UserExistsAsync(Guid id);
    // Task<bool> EmailExistsAsync(string email);
}