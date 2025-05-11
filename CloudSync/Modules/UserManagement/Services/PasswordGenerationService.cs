using PasswordGenerator;

namespace CloudSync.Modules.UserManagement.Services;

public struct PasswordAndHash
{
    public string GeneratedPassword;
    public string HashedPassword;
}

public class PasswordGenerationService
{
    public static PasswordAndHash GeneratePasswordAndHash()
    {
        var pwd = new Password();
        var generatedPassword = pwd.Next();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

        return new PasswordAndHash
        {
            GeneratedPassword = generatedPassword,
            HashedPassword = hashedPassword
        };
    }
}