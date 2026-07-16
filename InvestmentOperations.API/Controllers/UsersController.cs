using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                var result = _userService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An Error occured while listing the data");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _userService.GetById(id);
                if ( result == null)
                {
                    return NotFound("User noy found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured while retrieving the data: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Add(User user)
        {
            try
            {
                _userService.Add(user);
                return Ok("The User has been added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(User user)
        {
            try
            {
                _userService.Update(user);
                return Ok("The User has ben successfully updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _userService.GetById(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                _userService.Delete(id);
                return Ok("The User has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
