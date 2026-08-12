using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        public UsersController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get(UserQueryDto dto)
        {
            bool isAdmin = User.IsInRole("Admin");
            if (dto == null || dto.Id == null)
            {
                if (!isAdmin)
                {
                    return Forbid();
                }

                var result = await _userService.GetAll();
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                var userDtos = result.Data.Select(MapToDto).ToList();
                return Ok(userDtos);
            }
            else
            {
               var authResult = await _authorizationService.AuthorizeAsync(User, dto.Id.Value, "SameUserOrAdmin");
               if(!authResult.Succeeded)
                {
                    return Forbid();
                }
                var singleResult = await _userService.GetById(dto.Id.Value);
                if (!singleResult.Success)
                {
                    return BadRequest(singleResult.Message);
                }
                return Ok(MapToDto(singleResult.Data));
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserForUpdateDto dto)
        {
            var authResult= await _authorizationService.AuthorizeAsync(User, dto.UserId,"SameUserOrAdmin");
            if(!authResult.Succeeded)
            {
                return Forbid();
            }
            var user = new User
            {
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                IsActive = dto.IsActive

            };

            var result = await _userService.Update(user);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
