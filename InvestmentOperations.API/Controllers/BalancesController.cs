using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;

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
            var result = _balanceService.GetAllDetailed();
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

            return Ok(result);
        }



        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(int userId)
        {
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
            var result = _balanceService.Deposit(dto.UserId, dto.Amount);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
