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

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

}