using System.ComponentModel.DataAnnotations;

namespace user.dto;

//--------------------------- Authentication ----------------------------
public class LoginRequest
{
  [Required]
  [StringLength(50)]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
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
  [Required]
  [StringLength(50)]
  public string Name { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(150)]
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

