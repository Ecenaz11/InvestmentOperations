using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly IPriceService _priceService;
        public PricesController(IPriceService priceService)
        {
            _priceService = priceService;
        }
       
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = _priceService.GetAll();
                return Ok(result);
            }
            catch ( Exception ex)
            {
                return BadRequest($"An error occured while listing the data:{ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _priceService.GetById(id);
                if(result ==null)
                {
                    return NotFound("Price not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(Price price)
        {
            try
            {
                _priceService.Add(price);
                return Ok("The Price has been successfully Added.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(Price price)
        {
            try
            {
                _priceService.Update(price);
                return Ok("The Price has been successfully updated.");
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
                var price = _priceService.GetById(id);
                if (price == null)
                {
                    return NotFound("Price not found.");
                }
                _priceService.Delete(id);
                return Ok("The Price has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
