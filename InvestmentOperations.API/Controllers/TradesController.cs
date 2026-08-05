using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using InvestmentOperations.Core.Utilities.Results;


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
            IDataResult<List<TradeDto>> result;
            if(User.IsInRole("Admin"))
            {
                result = _tradeService.GetAll();
            }
            else
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                result = _tradeService.GetByUserId(callerId);
            }
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
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(callerId!= result.Data.UserId)
                {
                    return Forbid();
                }
            }
            return Ok(result);
        }
       
        [HttpPost]
        public IActionResult Add(TradeForAddDto dto)
        {
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(callerId!= dto.UserId)
                {
                    return Forbid();
                }
            }
            var trade = new Trade
            {
                AssetId = dto.AssetId,
                UserId = dto.UserId,
                Quantity = dto.Quantity,
                TradeType = dto.TradeType,
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
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var existing = _tradeService.GetById(dto.TradeId);
                if (!existing.Success || existing.Data.UserId != callerId)
                {
                    return Forbid();
                }
            }
            var trade = new Trade
            {
                TradeId = dto.TradeId,
                AssetId = dto.AssetId,
                UserId = dto.UserId,
                Quantity = dto.Quantity,
                TradeType = dto.TradeType,
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
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var existing = _tradeService.GetById(id);
                if (!existing.Success || existing.Data.UserId != callerId)
                {
                    return Forbid();
                }
            }

            var result = _tradeService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
         
    }
}
