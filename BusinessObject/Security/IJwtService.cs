using BusinessObject.Models;

namespace DataAccess.Security
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user, string role);
    }
}
