

using user.dto;

namespace user.iservice;

public interface IUserService
{
    //--------------------------- Authentication ----------------------------
    Task<TokenResponse?> LoginAsync(LoginRequest loginRequest);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);

    //--------------------------- User ---------------------------------------
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetCurrentUserAsync(Guid id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest createUserDto);
    Task<bool> DeleteUserAsync(Guid id);
}