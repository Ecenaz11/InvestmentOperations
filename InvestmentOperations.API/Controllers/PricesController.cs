using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;

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
       
        [HttpPost("get")]
        public IActionResult Get(PriceQueryDto dto)
        {
            if(dto==null || dto.Id == null)
            {
                var allResult = _priceService.GetAll();
                if (!allResult.Success)
                {
                    return BadRequest(allResult.Message);
                }
                return Ok(allResult);
            }
            var result = _priceService.GetById(dto.Id.Value);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
