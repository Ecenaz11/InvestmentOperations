using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto dto)
        {
            var result = _userService.Login(dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var token = _tokenService.CreateToken(result.Data);
            return Ok(new { token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(UserForRegisterDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password
            };

            var result = _userService.Add(user);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}