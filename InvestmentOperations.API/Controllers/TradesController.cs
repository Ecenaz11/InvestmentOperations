using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _tradeService.GetAll();
            if (!result.Success)
            {
                return BadRequest(result.Message);  
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _tradeService.GetById(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
       
        [HttpPost]
        public IActionResult Add(TradeForAddDto dto)
        {
            var trade = new Trade
            {
                AssetId = dto.AssetId,
                UserId = dto.UserId,
                Amount = dto.Amount,
                TradeType = dto.TradeType,
                UnitPrice = dto.UnitPrice
            };

            var result = _tradeService.Add(trade);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update(TradeForUpdateDto dto)
        {
            var trade = new Trade
            {
                TradeId = dto.TradeId,
                AssetId = dto.AssetId,
                UserId = dto.UserId,
                Amount = dto.Amount,
                TradeType = dto.TradeType,
                UnitPrice = dto.UnitPrice
            };

            var result = _tradeService.Update(trade);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _tradeService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
         
    }
}
