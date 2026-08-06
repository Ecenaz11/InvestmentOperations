using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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

        [HttpPost("get")]
        public IActionResult Get(UserQueryDto dto)
        {
           bool isAdmin = User.IsInRole("Admin");
           var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
           if (dto==null || dto.Id ==null)
            {
                if(!isAdmin)
                {
                    return Forbid();
                }

                var result = _userService.GetAll();
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                var userDtos = result.Data.Select(MapToDto).ToList();
                return Ok(userDtos);
            }
            else
            {
                if(!isAdmin && dto.Id != callerId)
                {
                    return Forbid();
                }
                var singleResult = _userService.GetById(dto.Id.Value);
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
        public IActionResult Update(UserForUpdateDto dto)
        {
            if(!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(dto.UserId != callerId)
                {
                    return Forbid();
                }
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
        
            var result = _userService.Update(user);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
