using ClashRoyaleDomain;

namespace ClashRoyaleRepository.RepositoryInterfaces
{
    public interface IUserRepository
    {
        User GetUserByUserNameAndPassword(string userName, string password);
    }
}
