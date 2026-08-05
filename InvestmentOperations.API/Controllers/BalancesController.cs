using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        public BalancesController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IDataResult<List<BalanceDto>> result;
            if(User.IsInRole("Admin"))
            {
                result = _balanceService.GetAllDetailed();
            }
            else
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                result = _balanceService.GetByUserIdDetailed(callerId);
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
            var result = _balanceService.GetByIdDetailed(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            if(!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(callerId!= result.Data.UserId)
                {
                    return Forbid();
                }
            }

            return Ok(result);
        }



        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            if(!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(callerId!= userId)
                {
                    return Forbid();
                }
            }
            var result = _balanceService.GetByUserIdDetailed(userId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Add(BalanceForAddDto dto)
        {
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(dto.UserId != callerId)
                {
                    return Forbid();
                }
            }
            var balance = new Balance
            {
                UserId = dto.UserId,
                AssetId = dto.AssetId,
                Amount = dto.Amount
            };

            var result = _balanceService.Add(balance);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("deposit")]
        public IActionResult Deposit(BalanceForDepositDto dto)
        {
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if(dto.UserId != callerId)
                {
                    return Forbid();
                }
            }
            var result = _balanceService.Deposit(dto.UserId, dto.Amount);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
