using System.ComponentModel.DataAnnotations;

namespace user.model;

public class User
{
  public Guid Id { get; set; } = Guid.NewGuid();

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

  [StringLength(200)]
  public string? RefreshToken { get; set; } = string.Empty;

  public DateTime? RefreshTokenExpiryTime { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

}