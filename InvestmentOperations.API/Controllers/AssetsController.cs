using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _assetService.GetAll();
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _assetService.GetById(id);
            if (result.Success)
            {
                return Ok(result.Message);
            }

             return BadRequest(result.Message);

        }

        [HttpPost]
        public IActionResult Add(Asset asset)
        {
            var result = _assetService.Add(asset);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);

        }

        [HttpPut]
        public IActionResult Update(Asset asset)
        {
            var result = _assetService.Update(asset);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
      
        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _assetService.Delete(id);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
                
                
        }
    }
}
