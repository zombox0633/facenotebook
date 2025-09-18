using System.Text;
using System.Text.RegularExpressions;
using Konscious.Security.Cryptography;
using helper.ihashPassword;

namespace helper.hashPassword;

public class HashPassword: IHashPassword
{
  private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$");

  public bool IsValidPassword(string password)
  {
    return !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);
  }
  
  public async Task<string> HashPasswordAsync(string password)
  {
    var salt = new byte[32]; // 32 bytes
    using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    rng.GetBytes(salt);

    var passwordBytes = Encoding.UTF8.GetBytes(password);
    using var argon2 = new Argon2id(passwordBytes)
    {
      Salt = salt,
      DegreeOfParallelism = 8, // จำนวน threads
      MemorySize = 65536, // 64 MB
      Iterations = 4 // จำนวนรอบ
    };

    // Generate hash
    var hash = await argon2.GetBytesAsync(32); // 32 bytes
    var combined = new byte[salt.Length + hash.Length]; // 32 + 32 = 64 bytes
    Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
    Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

    // bytes → string
    return Convert.ToBase64String(combined); // ~88 characters
  }

  public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
  {
    try
    {
      // string → bytes
      var combined = Convert.FromBase64String(hashedPassword);

      var salt = new byte[32];
      var hash = new byte[32];
      Buffer.BlockCopy(combined, 0, salt, 0, 32);
      Buffer.BlockCopy(combined, 32, hash, 0, 32);

      var passwordBytes = Encoding.UTF8.GetBytes(password);
      using var argon2 = new Argon2id(passwordBytes)
      {
        Salt = salt,
        DegreeOfParallelism = 8,
        MemorySize = 65536,
        Iterations = 4
      };

      var newHash = await argon2.GetBytesAsync(32);
      return hash.SequenceEqual(newHash);
    }
    catch
    {
      return false;
    }
  }
}