using CoreAPI.Data.Models;

namespace CoreAPI.IRepository
{
    public interface IJWTRepository
    {
        string GenerateAccessToken(AspNetUser user, IList<string> roles);
    }
}
