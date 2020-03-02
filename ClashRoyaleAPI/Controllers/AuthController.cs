using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClashRoyaleService.ServiceInterfaces;
using ClashRoyaleUtils.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClashRoyaleAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<UserDTO> Post([FromBody] UserDTO userDTO)
        {
            var user = _userService.GetUserTokenByUser(userDTO.UserName, userDTO.Password);

            if (user == null)
                return NotFound($"Usuário {userDTO.UserName} não localizado!");

            return Ok(user);
        }
    }
}