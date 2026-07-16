using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                var result = _tradeService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured while listing the data:{ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _tradeService.GetById(id);
                if(result==null)
                {
                    return NotFound("Trade not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured while retrieving the data: {ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult Add(Trade trade)
        {
            try
            {
                _tradeService.Add(trade);
                return Ok("The Trade has been successfully Added.");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(Trade trade)
        {
            try
            {
                _tradeService.Update(trade);
                return Ok("The Trade has been successfully updated.");
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
                var trade = _tradeService.GetById(id);
                if(trade == null)
                {
                    return NotFound("Trade not found");
                }
                _tradeService.Delete(id);
                return Ok("The Trade has been successfully deleted.");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
         
    }
}
