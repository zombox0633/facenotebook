namespace helper.ihashPassword;

public interface IHashPassword
{
    bool IsValidPassword(string password);
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
}