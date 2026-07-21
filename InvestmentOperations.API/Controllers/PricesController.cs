using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using InvestmentOperations.Entities.Dtos;

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
            var result = _priceService.GetAll();
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _priceService.GetById(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Add(PriceForAddDto dto)
        {
            var price = new Price
            {
                AssetId = dto.AssetId,
                CurrentPrice = dto.CurrentPrice
            };

            var result = _priceService.Add(price);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        
        

        [HttpPut]
        public IActionResult Update(PriceForUpdateDto dto)
        {
            var price = new Price
            {
                PriceId = dto.PriceId,
                AssetId = dto.AssetId,
                CurrentPrice = dto.CurrentPrice
            };
        
            var result = _priceService.Update(price);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _priceService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
