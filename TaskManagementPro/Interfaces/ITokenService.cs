using TaskManagementPro.Models;

namespace TaskManagementPro.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string token);
    }
}
