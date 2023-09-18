using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProvidersDomain.Models;
using ProvidersDomain.Models.ApiModels;
using ProvidersDomain.Services;

namespace ProdmasterProvidersApi.Controllers
{
    /// <summary>
    /// Porviders api
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ProvidersApiController : ControllerBase
    {        

        private readonly ILogger<ProvidersApiController> _logger;
        private readonly ISpecificationApiService _specificationService;
        private readonly IUpdateProvidersService _updateProvidersService;

        public ProvidersApiController(ILogger<ProvidersApiController> logger, ISpecificationApiService specificationService, IUpdateProvidersService updateProvidersService)
        {
            _logger = logger;
            _specificationService = specificationService;
            _updateProvidersService = updateProvidersService;
        }
        /// <summary>
        /// Get new specifications
        /// </summary>
        /// <returns></returns>
        [HttpGet("specifications")]
        [ProducesResponseType(typeof(IEnumerable<SpecificationApiModel>), 200)]
        public async Task<IEnumerable<SpecificationApiModel>> GetSpecifications()
        {
            return await _specificationService.GetNewSpecifications();
        }
        /// <summary>
        /// Success recieving specifications
        /// </summary>
        /// <param name="specificationIds"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPatch("specificationsToVerify")]
        public async Task<IActionResult> GetSpecificationsSuccess([FromBody]List<long> specificationIds)
        {
            if (!await _specificationService.SuccessSendingSpecifications(specificationIds))
                return BadRequest("Error");
            return Ok();
        }
        /// <summary>
        /// Update specifications
        /// </summary>
        /// <param name="specificationList"></param>
        /// <returns></returns>
        [HttpPut("specifications")]
        public async Task<IActionResult> UpdateSpecifications([FromBody]IEnumerable<UpdateSpecificationApiModel> specificationList)
        {
            try
            {
                await _specificationService.AddOrUpdateSpecifications(specificationList);
                return Ok();
            }
            catch
            {
                _logger.LogError("Failed to update specifications");
                return BadRequest("Failed to update specifications");
            }
        }
        [HttpPost("provider")]
        public async Task<IActionResult> LoadProvider([FromBody]long customId, string? password)
        {
                await _updateProvidersService.LoadProvider(customId, password);
                return Ok();
            //try
            //{
            //    await _updateProvidersService.LoadProvider(customId);
            //    return Ok();
            //}
            //catch
            //{
            //    _logger.LogError("Failed to load provider");
            //    return BadRequest("Failed to load provider");
            //}
        }
        [HttpGet("provider")]
        public async Task<IActionResult> LoadProviders()
        {
            await _updateProvidersService.LoadProviders();
            return Ok();
        }
    }
}