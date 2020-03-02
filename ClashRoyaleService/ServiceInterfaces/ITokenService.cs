using ClashRoyaleDomain;

namespace ClashRoyaleService.ServiceInterfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
