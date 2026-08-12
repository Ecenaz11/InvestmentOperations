using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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

        [HttpPost("get")]
        public async Task<IActionResult> Get(AssetQueryDto dto)
        {
            if (dto == null || dto.Id == null)
            {
                var allResult = await _assetService.GetAll();
                if (!allResult.Success)
                {
                    return BadRequest(allResult.Message);
                }
                return Ok(allResult);
            }
            var result = await _assetService.GetById(dto.Id.Value);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AssetForAddDto dto)
        {
            var asset = new Asset
            {
                AssetName = dto.AssetName,
                AssetType = dto.AssetType,
                AssetCode = dto.AssetCode
            };

            var result = await _assetService.Add(asset);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);

        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(AssetForUpdateDto dto)
        {
            var asset = new Asset
            {
                AssetId = dto.AssetId,
                AssetName = dto.AssetName,
                AssetType = dto.AssetType,
                AssetCode = dto.AssetCode
            };

            var result = await _assetService.Update(asset);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assetService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);


        }
    }
}
