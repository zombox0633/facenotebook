using System.Security.Claims;
using user.model;

namespace utils.ijwt;

public interface IJwtService
{
  string GenerateAccessToken(User user);
  // string GenerateRefreshToken();
  // DateTime GetAccessTokenExpiry();
  // DateTime GetRefreshTokenExpiry();
  // bool ValidateToken(string token);
  // ClaimsPrincipal GetPrincipalFromToken(string token);
}