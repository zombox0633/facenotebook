using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using user.model;
using helper.ijwt;

namespace helper.jwt;

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

  public string GenerateRefreshToken()
  {
    var randomBytes = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);
    return Convert.ToBase64String(randomBytes);
  }

  //--------------------- TokenExpiry -------------------
  public ClaimsPrincipal ExtractUserFromExpiredToken(string expiredToken)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_secretKey);

    try
    {
      var extract = tokenHandler.ValidateToken(expiredToken, new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = _issuer,
        ValidateAudience = true,
        ValidAudience = _audience,
        ValidateLifetime = false,
        ClockSkew = TimeSpan.Zero
      }, out _);
      
      return extract;
    }
    catch
    {
      throw new SecurityTokenException("Invalid token format");
    }
  }

  public DateTime GetAccessTokenExpiry()
  {
    var utcTime = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);
    return DateTime.SpecifyKind(utcTime, DateTimeKind.Unspecified);
  }

  public DateTime GetRefreshTokenExpiry()
  {
    var utcTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
    return DateTime.SpecifyKind(utcTime, DateTimeKind.Unspecified);
  }
}