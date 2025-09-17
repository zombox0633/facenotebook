using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user.dto;
using user.iservice;

namespace controller.authController;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly ILogger<AuthController> _logger;

  public AuthController(IUserService userService, ILogger<AuthController> logger)
  {
    _userService = userService;
    _logger = logger;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var result = await _userService.LoginAsync(loginRequest);

      if (result == null)
        return Unauthorized(new { message = "Invalid email or password" });

      _logger.LogInformation("User {Email} logged in successfully", loginRequest.Email);
      return Ok(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred during login for {Email}", loginRequest.Email);
      return StatusCode(500, "An error occurred while processing your request");
    }
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
  {
    var result = await _userService.RefreshTokenAsync(refreshRequest.RefreshToken);

    if (result == null)
      return Unauthorized(new { message = "Invalid refresh token" });

    return Ok(result);
  }

  [HttpPost("logout")]
  // [Authorize]
  public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest refreshRequest)
  {
    await _userService.LogoutAsync(refreshRequest.RefreshToken);
    return Ok(new { message = "Logged out successfully" });
  }
}