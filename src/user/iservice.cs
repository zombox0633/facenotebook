

using user.dto;

namespace user.iservice;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(Guid id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest createUserDto);
    Task<bool> DeleteUserAsync(Guid id);
}