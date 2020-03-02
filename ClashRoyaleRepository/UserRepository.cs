using ClashRoyaleDomain;
using ClashRoyaleRepository.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace ClashRoyaleRepository
{
    public sealed class UserRepository : IUserRepository
    {
        public User GetUserByUserNameAndPassword(string userName, string password)
        {
            var users = new List<User>();
            users.Add(new User()
            {
                Id = 1,
                UserName = "MMarcos",
                Password = "123456",
                Role = "Admin"
            });

            users.Add(new User()
            {
                Id = 2,
                UserName = "HugoAdim",
                Password = "654321",
                Role = "Admin"
            });

            users.Add(new User()
            {
                Id = 2,
                UserName = "HugoUser",
                Password = "654321",
                Role = "User"
            });

            var user = users.FirstOrDefault(u => u.UserName == userName && u.Password == password);
            user.Password = string.Empty;

            return user;
           
        }
    }
}
