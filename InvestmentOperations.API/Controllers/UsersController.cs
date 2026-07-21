using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
           var result = _userService.GetAll();
           if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            
            var userDto = new List<UserDto>();
            foreach (var user in result.Data)
            {
                var dto = new UserDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };
                userDto.Add(dto);
            }
            return Ok(userDto);
        }

        

        [HttpGet("{id}")]
        public IActionResult Get(int id )
        {
            var result = _userService.GetById(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            var userDto = new UserDto
            {
                UserId = result.Data.UserId,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                IsActive = result.Data.IsActive,
                CreatedAt = result.Data.CreatedAt
            };
            return Ok(userDto);
        }

        [HttpPost]
        public IActionResult Add(UserForRegisterDto dto)
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

        [HttpPut]
        public IActionResult Update(UserForUpdateDto dto)
        {
            var user = new User
            {
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                IsActive = dto.IsActive

            };
        
            var result = _userService.Update(user);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _userService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

    }
}
