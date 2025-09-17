using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user.dto;
using user.iservice;

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
  public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
  {
    try
    {
      var users = await _userService.GetAllUsersAsync();
      return Ok(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting users");
      return StatusCode(500, "An error occurred while processing your request");
    }
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<UserResponse>> GetUser(Guid id)
  {
    try
    {
      var user = await _userService.GetCurrentUserAsync(id);
      if (user == null)
      {
        return NotFound($"User with ID {id} not found");
      }

      return Ok(user);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting user {UserId}", id);
      return StatusCode(500, "An error occurred while processing your request");
    }
  }

  [HttpPost]
  public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest createUserDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var user = await _userService.CreateUserAsync(createUserDto);
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
  }

  [HttpDelete("{id}")]
  [Authorize]
  public async Task<IActionResult> DeleteUser(Guid id)
  {
    var deleted = await _userService.DeleteUserAsync(id);
    if (!deleted)
      return NotFound($"User with ID {id} not found");

    return NoContent();
  }
}