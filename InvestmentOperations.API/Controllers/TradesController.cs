using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.API.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        private readonly IAuthorizationService _authorizationService;
        public TradesController(ITradeService tradeService, IAuthorizationService authorizationService)
        {
            _tradeService = tradeService;
            _authorizationService = authorizationService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get(TradeQueryDto dto)
        {
            bool isAdmin = User.IsInRole("Admin");
            var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            if (dto == null || dto.Id == null)
            {
                IDataResult<List<TradeDto>> result = isAdmin ? _tradeService.GetAll() : _tradeService.GetByUserId(callerId);
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            else
            {
                var result = _tradeService.GetById(dto.Id.Value);
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                var authResult = await _authorizationService.AuthorizeAsync(User, result.Data.UserId, "SameUserOrAdmin");
                if(!authResult.Succeeded)
                {
                    return Forbid();
                }
                return Ok(result);
            }
        }

        [HttpPost]
        public IActionResult Add(TradeForAddDto dto)
        {
            int targetUserId = dto.UserId;
            if(!User.IsInRole ("Admin"))
            {
               targetUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
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
        public async Task<IActionResult> Update(TradeForUpdateDto dto)
        {
            var existing = _tradeService.GetById(dto.TradeId);
            if(!existing.Success)
            {
                return BadRequest(existing.Message);
            }
            var authResult = await _authorizationService.AuthorizeAsync(User,dto.UserId, "SameUserOrAdmin");
            if(!authResult.Succeeded)
            {
                return Forbid();
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
        public async Task<IActionResult> Delete(int id)
        {
            var existing = _tradeService.GetById(id);
            if(!existing.Success)
            {
                return BadRequest(existing.Message);
            }
            var authResult = await _authorizationService.AuthorizeAsync(User,existing.Data.UserId, "SameUserOrAdmin");
            if(!authResult.Succeeded)
            {
                return Forbid();
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
