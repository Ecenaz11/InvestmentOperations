using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.API.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration.UserSecrets;


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
                IDataResult<List<TradeDto>> result = isAdmin ? await _tradeService.GetAll() : await _tradeService.GetByUserId(callerId);
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            else
            {
                var result = await _tradeService.GetById(dto.Id.Value);
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
        public async Task<IActionResult> Add(TradeForAddDto dto)
        {
            int targetUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var trade = new Trade
            {
                AssetId = dto.AssetId,
                UserId = targetUserId,
                Quantity = dto.Quantity,
                TradeType = dto.TradeType,
            };

            var result = await _tradeService.Add(trade);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
