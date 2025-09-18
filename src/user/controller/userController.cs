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
    try
    {
      var users = await _userService.GetAllUsersAsync();
      return apiFormatResponse.Success(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting users");
      return StatusCode(500, "An error occurred while processing your request");
    }
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<ApiFormatResponse<UserResponse>>> GetUserById(Guid id)
  {
    try
    {
      var user = await _userService.GetCurrentUserAsync(id);
      if (user == null)
      return NotFound($"User with ID {id} not found");

      return apiFormatResponse.Success(user);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting user {UserId}", id);
      return StatusCode(500, "An error occurred while processing your request");
    }
  }

  [HttpPost]
  public async Task<ActionResult<ApiFormatResponse<UserResponse>>> CreateUser(CreateUserRequest createUserDto)
  {
    if (!ModelState.IsValid)
      throw new InvalidOperationException("Invalid user data");

    var user = await _userService.CreateUserAsync(createUserDto);
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