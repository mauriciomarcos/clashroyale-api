using ClashRoyaleUtils.DataTransferObjects;

namespace ClashRoyaleService.ServiceInterfaces
{
    public interface IUserService
    {
        UserDTO GetUserTokenByUser(string userName, string password);
    }
}
