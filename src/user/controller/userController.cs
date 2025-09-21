using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user.dto;
using user.iservice;
using utils.apiFormatResponse;

namespace controller.userController;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly ILogger<UsersController> _logger;

  public UsersController(IUserService userService, ILogger<UsersController> logger)
  {
    _userService = userService;
    _logger = logger;
  }

  [HttpGet]
  [Authorize]
  public async Task<ActionResult<ApiFormatResponse<IEnumerable<UserResponse>>>> GetUsers()
  {
    var users = await _userService.GetAllUsersAsync();
    return apiFormatResponse.Success(users);
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<ApiFormatResponse<UserResponse>>> GetUserById(Guid id)
  {
    var user = await _userService.GetCurrentUserAsync(id);
    if (user == null)
    throw new KeyNotFoundException($"User with ID {id} not found");

    return apiFormatResponse.Success(user);
  }

  [HttpPost]
  public async Task<ActionResult<ApiFormatResponse<UserResponse>>> CreateUser(CreateUserRequest createUser)
  {
    if (!ModelState.IsValid)
      return apiFormatResponse.ValidationError<UserResponse>(ModelState, "/api/users");

    var user = await _userService.CreateUserAsync(createUser);
    return apiFormatResponse.Success(user, 201, "User created successfully");
  }

  [HttpDelete("{id}")]
  [Authorize]
  public async Task<ActionResult<ApiFormatResponse<object?>>> DeleteUser(Guid id)
  {
    var deleted = await _userService.DeleteUserAsync(id);
    if (!deleted)
      throw new KeyNotFoundException($"User with ID {id} not found");

    return apiFormatResponse.Success<object?>(null, 204, "User deleted successfully");
  }
}