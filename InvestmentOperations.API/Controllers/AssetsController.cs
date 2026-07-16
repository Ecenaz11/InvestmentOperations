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
           try
            {
                var result = _assetService.GetAll();
                return Ok(result);
            }

            catch(Exception ex)
            {
                return BadRequest($"An error occured while listing the data:{ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
           try
            {
                var result = _assetService.GetById(id);
                if (result==null)
                {
                    return NotFound("Asset not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured while retrieving the data: {ex.Message}");
            }
            
        }

        [HttpPost]
        public IActionResult Add(Asset asset)
        {
            try
            {
                _assetService.Add(asset);
                return Ok("The Asset has been added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut]
        public IActionResult Update(Asset asset)
        {
            try
            {
                _assetService.Update(asset);
                return Ok("The Asset has been successfully updated.");
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
                var asset = _assetService.GetById(id);
                if (asset == null)
                {
                    return NotFound("Asset not found.");
                }
                _assetService.Delete(id);
                return Ok("The Asset has been successfully deleted.");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
                
        }
    }
}
