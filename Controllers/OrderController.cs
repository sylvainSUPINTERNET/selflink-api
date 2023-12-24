using Microsoft.AspNetCore.Mvc;
using Selflink_api.Db.Models;
using Selflink_api.Dto;
using Selflink_api.Services;

namespace Selflink_api.Controllers;

    [ApiController]
    [Route("api/orders")]
    public class OrderController: ControllerBase
    {

        private readonly ILogger<OrderController> _logger;

        private readonly IOrderService _orderService;

        public OrderController( ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost(Name = "GetUserOrders ")]

        public async Task<ActionResult<Order>> GetListAsync (OrderListCreateria orderListCriteria)
        {
            _logger.LogInformation("GetUserOrders triggered");

            try {
                return Ok(await _orderService.GetOrdersAsync(orderListCriteria.StripeProductId, orderListCriteria.IdLast, orderListCriteria.Limit));
            } catch ( Exception e ) {
                return BadRequest(e.Message);
            }
        }



    }
