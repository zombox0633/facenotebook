using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user.dto;
using user.iservice;
using utils.apiFormatResponse;

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
  public async Task<ActionResult<ApiFormatResponse<TokenResponse>>> Login([FromBody] LoginRequest loginRequest) 
  {
    if (!ModelState.IsValid)
      return apiFormatResponse.ValidationError<TokenResponse>(ModelState, "/api/auth/login");

    var result = await _userService.LoginAsync(loginRequest);

    if (result == null)
      throw new UnauthorizedAccessException("Invalid email or password");

    _logger.LogInformation("User {Email} logged in successfully", loginRequest.Email);
    return apiFormatResponse.Success(result, 200, "Login successful");
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<ApiFormatResponse<TokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
  {
    if (!ModelState.IsValid)
      return apiFormatResponse.ValidationError<TokenResponse>(ModelState, "/api/auth/refresh");
    
    var result = await _userService.RefreshTokenAsync(refreshRequest.RefreshToken);

    if (result == null)
      throw new UnauthorizedAccessException("Invalid or expired refresh token");

    return apiFormatResponse.Success(result, 200, "Token refreshed successfully");
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<ActionResult<ApiFormatResponse<object?>>> Logout([FromBody] RefreshTokenRequest refreshRequest)
  {
    if (!ModelState.IsValid)
      return apiFormatResponse.ValidationError<object?>(ModelState, "/api/auth/logout");

    var success = await _userService.LogoutAsync(refreshRequest.RefreshToken);

    if (!success)
      throw new InvalidOperationException("Logout failed - invalid refresh token");
    
    return apiFormatResponse.Success<object?>(null, 200, "Logged out successfully");
  }
}