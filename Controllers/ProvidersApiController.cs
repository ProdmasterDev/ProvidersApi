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
        private readonly IOrderService _orderService;

        public ProvidersApiController(ILogger<ProvidersApiController> logger, ISpecificationApiService specificationService, IUpdateProvidersService updateProvidersService, IOrderService orderService)
        {
            _logger = logger;
            _specificationService = specificationService;
            _updateProvidersService = updateProvidersService;
            _orderService = orderService;
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
        public async Task<IActionResult> GetSpecificationsSuccess([FromBody]List<ConfirmSpecificationApiModel> specs)
        {
            if (!await _specificationService.ConfirmSendingSpecifications(specs))
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
        }
        [HttpGet("provider")]
        public async Task<IActionResult> LoadProviders()
        {
            await _updateProvidersService.LoadProviders();
            return Ok();
        }
        [HttpPost("createorder")]
        public async Task<OrderApiResponseModel?> CreateOrder([FromBody] OrderApiModel order)
        {
            var createdOrder = await _orderService.CreateOrRecreateOrder(order);
            if (createdOrder.Id == default)
            {
                return new OrderApiResponseModel();
            }
            else
            {
                return new OrderApiResponseModel(createdOrder);
            }
        }
        [HttpPost("getordersforuser")]
        public async Task<List<Order>> GetOrdersForUser(long userId)
        {
            return await _orderService.GetOrdersForUser(userId);
        }
        [HttpPost("getorders")]
        public async Task<List<OrderApiModel>> GetOrders()
        {
            return await _orderService.GetConfirmedOrDeclinedOrders();
        }
        [HttpPost("approveorders")]
        public async Task ApproveOrders([FromBody] List<OrderApiModel> orders)
        {
            await _orderService.ConfirmConfirmedOrDeclinedOrders(orders);
        }
        [HttpPost("declineorder")]
        public async Task DeclineOrder([FromBody] OrderApiModel order)
        {
            await _orderService.DeclineOrderByRecipient(order);
        }
    }
}