using System.Security.Claims;
using user.model;

namespace utils.ijwt;

public interface IJwtService
{
  string GenerateAccessToken(User user);
  string GenerateRefreshToken();

  //--------------------- TokenExpiry -------------------
  ClaimsPrincipal ExtractUserFromExpiredToken(string expiredToken);
  DateTime GetAccessTokenExpiry();
  DateTime GetRefreshTokenExpiry();

}