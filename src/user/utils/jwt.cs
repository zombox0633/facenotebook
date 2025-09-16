using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using user.model;
using utils.ijwt;

namespace utils.jwt;

public class JwtService : IJwtService
{
  private readonly string _secretKey;
  private readonly string _issuer = "facenotebook_app";
  private readonly string _audience = "earth_ten";
  private readonly int _accessTokenExpirationMinutes = 15;
  private readonly int _refreshTokenExpirationDays = 7;

  public JwtService(IConfiguration configuration)
  {
    _secretKey = configuration["Jwt:SecretKey"] ??
      throw new ArgumentNullException("Jwt:SecretKey not found");
  }

  public string GenerateAccessToken(User user)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Email, user.Email),
      new Claim(ClaimTypes.Name, user.Name)
    };

    var token = new JwtSecurityToken(
      issuer: _issuer,
      audience: _audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}