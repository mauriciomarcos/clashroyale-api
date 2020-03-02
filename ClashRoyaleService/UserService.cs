using ClashRoyaleDomain;
using ClashRoyaleRepository.RepositoryInterfaces;
using ClashRoyaleService.ServiceInterfaces;
using ClashRoyaleUtils.DataTransferObjects;
using System;

namespace ClashRoyaleService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public UserDTO GetUserTokenByUser(string userName, string password)
        {
            var user = _userRepository.GetUserByUserNameAndPassword(userName, password);

            if (user != null)
            {
                var token = _tokenService.GenerateToken(user);

                var userDTO = new UserDTO()
                {
                    UserName = userName,
                    Token = token
                };

                return userDTO;
            }

            return default(UserDTO);
        }
    }
}
