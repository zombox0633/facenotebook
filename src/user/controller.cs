using Microsoft.AspNetCore.Mvc;
using user.dto;
using user.iservice;

namespace user.controller;

[ApiController]
[Route("api/user")]
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
  public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
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
  public async Task<ActionResult<UserResponseDto>> GetUser(Guid id)
  {
    try
    {
      var user = await _userService.GetUserByIdAsync(id);
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
  

}