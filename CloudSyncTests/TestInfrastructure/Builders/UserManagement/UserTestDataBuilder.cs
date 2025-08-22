using CloudSync.Modules.UserManagement.Models;

namespace Tests.TestInfrastructure.Builders.UserManagement;

public class UserTestDataBuilder
{
    private int _id = 1;
    private string _googleUserId = "default-google-id";
    private string _email = "test@example.com";
    private DateTime _createDateTime = DateTime.UtcNow.AddDays(-30);
    private DateTime _lastLoginDateTime = DateTime.UtcNow.AddDays(-1);
    private string? _userSettings = null;

    public UserTestDataBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UserTestDataBuilder WithGoogleUserId(string googleUserId)
    {
        _googleUserId = googleUserId;
        return this;
    }

    public UserTestDataBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserTestDataBuilder WithCreateDateTime(DateTime createDateTime)
    {
        _createDateTime = createDateTime;
        return this;
    }

    public UserTestDataBuilder WithLastLoginDateTime(DateTime lastLoginDateTime)
    {
        _lastLoginDateTime = lastLoginDateTime;
        return this;
    }

    public UserTestDataBuilder WithUserSettings(string? userSettings)
    {
        _userSettings = userSettings;
        return this;
    }

    public UserTestDataBuilder CreatedDaysAgo(int days)
    {
        _createDateTime = DateTime.UtcNow.AddDays(-days);
        return this;
    }

    public UserTestDataBuilder LastLoginDaysAgo(int days)
    {
        _lastLoginDateTime = DateTime.UtcNow.AddDays(-days);
        return this;
    }

    public User Build()
    {
        return new User
        {
            Id = _id,
            GoogleUserId = _googleUserId,
            Email = _email,
            CreateDateTime = _createDateTime,
            LastLoginDateTime = (DateTime)_lastLoginDateTime,
            UserSettings = _userSettings
        };
    }
}