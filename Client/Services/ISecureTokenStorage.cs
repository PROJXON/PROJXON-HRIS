using System.Threading.Tasks;

namespace Client.Services;

public interface ISecureTokenStorage
{
    Task StoreTokenAsync(string key, string token);
    Task<string?> GetTokenAsync(string key);
    Task DeleteTokenAsync(string key);
}