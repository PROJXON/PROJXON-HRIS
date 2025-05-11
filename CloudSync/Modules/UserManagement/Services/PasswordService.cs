using PasswordGenerator;

namespace CloudSync.Modules.UserManagement.Services;

public struct PasswordAndHash
{
    public string GeneratedPassword;
    public string HashedPassword;
}

public class PasswordService
{
    public static PasswordAndHash GeneratePasswordAndHash()
    {
        var pwd = new Password();
        var generatedPassword = pwd.Next();

        var hashedPassword = HashPassword(generatedPassword);

        return new PasswordAndHash
        {
            GeneratedPassword = generatedPassword,
            HashedPassword = hashedPassword
        };
    }

    public static string HashPassword(string generatedPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(generatedPassword);
    }
}