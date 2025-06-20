using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudSync.Modules.UserManagement.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Tests.Unit.UserManagement.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _tokenService;
    private const string TestKey = "this-is-a-very-long-test-key-that-meets-minimum-requirements-for-hmac-sha256";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";
    private const string TestExpiresInMinutes = "60";
    private const string TestEmail = "test@example.com";

    public JwtTokenServiceTests()
    {
        var configData = new Dictionary<string, string>
        {
            {"JWT:Key", TestKey},
            {"JWT:Issuer", TestIssuer},
            {"JWT:Audience", TestAudience},
            {"JWT:ExpiresInMinutes", TestExpiresInMinutes}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        _tokenService = new JwtTokenService(configuration);
    }

    [Fact]
    public void GenerateToken_WithValidEmail_ReturnsValidJwtToken()
    {
        // Act
        var token = _tokenService.GenerateToken(TestEmail);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        
        // Verify it's a valid JWT format (3 parts separated by dots)
        var tokenParts = token.Split('.');
        Assert.Equal(3, tokenParts.Length);
    }

    [Fact]
    public void GenerateToken_WithValidEmail_TokenContainsCorrectClaims()
    {
        // Act
        var token = _tokenService.GenerateToken(TestEmail);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);
        
        Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == TestEmail);
    }

    [Fact]
    public void GenerateToken_WithValidEmail_TokenHasCorrectIssuerAndAudience()
    {
        // Act
        var token = _tokenService.GenerateToken(TestEmail);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);
        
        Assert.Equal(TestIssuer, jsonToken.Issuer);
        Assert.Contains(TestAudience, jsonToken.Audiences);
    }

    [Fact]
    public void GenerateToken_WithValidEmail_TokenHasCorrectExpiration()
    {
        // Arrange
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _tokenService.GenerateToken(TestEmail);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);
        
        var expectedExpiry = beforeGeneration.AddMinutes(double.Parse(TestExpiresInMinutes));
        var timeDifference = Math.Abs((jsonToken.ValidTo - expectedExpiry).TotalSeconds);
        Assert.True(timeDifference < 5, $"Token expiry time differs by {timeDifference} seconds");
    }

    [Fact]
    public void GenerateToken_WithValidEmail_TokenCanBeValidatedWithSameKey()
    {
        // Act
        var token = _tokenService.GenerateToken(TestEmail);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = TestIssuer,
            ValidateAudience = true,
            ValidAudience = TestAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Should not throw exception
        var exception = Record.Exception(() => tokenHandler.ValidateToken(token, validationParameters, out _));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void GenerateToken_WithInvalidEmail_StillGeneratesToken(string email)
    {
        // Act & Assert - the method doesn't validate email format, just uses it as a claim
        var exception = Record.Exception(() => _tokenService.GenerateToken(email));
        Assert.Null(exception);
    }

    [Fact]
    public void GenerateToken_WithMissingJwtKey_ThrowsConfigurationException()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            {"JWT:Issuer", TestIssuer},
            {"JWT:Audience", TestAudience},
            {"JWT:ExpiresInMinutes", TestExpiresInMinutes}
            // Missing JWT:Key
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var tokenService = new JwtTokenService(configuration);

        // Act & Assert
        var exception = Assert.Throws<ConfigurationException>(() => tokenService.GenerateToken(TestEmail));
        Assert.Equal("Jwt key not found or missing.", exception.Message);
    }

    [Fact]
    public void GenerateToken_WithMissingExpiresInMinutes_ThrowsConfigurationException()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            {"JWT:Key", TestKey},
            {"JWT:Issuer", TestIssuer},
            {"JWT:Audience", TestAudience}
            // Missing JWT:ExpiresInMinutes
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var tokenService = new JwtTokenService(configuration);

        // Act & Assert
        var exception = Assert.Throws<ConfigurationException>(() => tokenService.GenerateToken(TestEmail));
        Assert.Equal("Expiration time not found or missing.", exception.Message);
    }

    [Fact]
    public void GenerateToken_WithInvalidExpiresInMinutes_ThrowsFormatException()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            {"JWT:Key", TestKey},
            {"JWT:Issuer", TestIssuer},
            {"JWT:Audience", TestAudience},
            {"JWT:ExpiresInMinutes", "not-a-number"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var tokenService = new JwtTokenService(configuration);

        // Act & Assert
        Assert.Throws<FormatException>(() => tokenService.GenerateToken("test@example.com"));
    }

    [Fact]
    public void GenerateToken_CalledMultipleTimes_GeneratesDifferentTokens()
    {
        // Act
        var token1 = _tokenService.GenerateToken(TestEmail);
        var token2 = _tokenService.GenerateToken(TestEmail);

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateToken_WithDifferentEmails_GeneratesDifferentTokens()
    {
        // Arrange
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";

        // Act
        var token1 = _tokenService.GenerateToken(email1);
        var token2 = _tokenService.GenerateToken(email2);

        // Assert
        Assert.NotEqual(token1, token2);
        
        // Verify different claims
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken1 = tokenHandler.ReadJwtToken(token1);
        var jsonToken2 = tokenHandler.ReadJwtToken(token2);
        
        var email1Claim = jsonToken1.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var email2Claim = jsonToken2.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        
        Assert.Equal(email1, email1Claim);
        Assert.Equal(email2, email2Claim);
    }
}