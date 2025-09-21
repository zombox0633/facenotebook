using System.ComponentModel.DataAnnotations;

namespace user.dto;

//--------------------------- Authentication ----------------------------
public class LoginRequest
{
  [Required(ErrorMessage = "Email is required")]
  [EmailAddress(ErrorMessage = "Invalid email format")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "Password is required")]
  [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
  public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
  [Required]
  public string RefreshToken { get; set; } = string.Empty;
}

public class TokenResponse
{
  public string AccessToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
}

//------------------------- User ------------------------------------------
public class CreateUserRequest
{
  [Required(ErrorMessage = "Name is required")]
  public string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "Email is required")]
  [EmailAddress(ErrorMessage = "Invalid email format")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "Password is required")]
  [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
  public string Password { get; set; } = string.Empty;
}

public class UserResponse
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}

