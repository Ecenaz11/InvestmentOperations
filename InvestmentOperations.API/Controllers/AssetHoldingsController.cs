using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetHoldingsController : ControllerBase
    {
        private readonly IAssetHoldingService _assetHoldingService;
        private readonly IAuthorizationService _authorizationService;
        public AssetHoldingsController(IAssetHoldingService assetHoldingService, IAuthorizationService authorizationService)
        {
            _assetHoldingService = assetHoldingService;
            _authorizationService = authorizationService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get(AssetHoldingQueryDto dto)
        {
            var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            bool isAdmin = User.IsInRole("Admin");

            if (dto != null && dto.Id != null)
            {
                var result = await _assetHoldingService.GetByIdDetailed(dto.Id.Value);
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

            if (dto != null && dto.UserId != null)
            {
                var authResult = await _authorizationService.AuthorizeAsync(User, dto.UserId.Value, "SameUserOrAdmin");
                if(!authResult.Succeeded)
                {
                    return Forbid();
                }

                var userResult = await _assetHoldingService.GetByUserIdDetailed(dto.UserId.Value);
                if (!userResult.Success)
                {
                    return BadRequest(userResult.Message);
                }
                return Ok(userResult);
            }

            var allResult = isAdmin ? await _assetHoldingService.GetAllDetailed() : await _assetHoldingService.GetByUserIdDetailed(callerId);
            if (!allResult.Success)
            {
                return BadRequest(allResult.Message);
            }
            return Ok(allResult);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AssetHoldingForAddDto dto)
        {
            int targetUserId = dto.UserId;
            if(!User.IsInRole("Admin"))
            {
                targetUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            }

            var assetHolding = new AssetHolding
            {
                UserId = dto.UserId,
                AssetId = dto.AssetId,
                Amount = dto.Amount
            };

            var result = await _assetHoldingService.Add(assetHolding);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDto dto)
        {
            int targetUserId = dto.UserId;
            if(!User.IsInRole("Admin"))
            {
                targetUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            }
            var result = await _assetHoldingService.Deposit(dto.UserId, dto.Amount);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
