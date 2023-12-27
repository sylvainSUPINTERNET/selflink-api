using Microsoft.AspNetCore.Mvc;
using Selflink_api.Db.Models;
using Selflink_api.Dto;
using Selflink_api.Dto.Api;
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


        [HttpPost("refund", Name = "RefundOrder")]
        public async Task<ActionResult> RefundOrderAsync(OrderRefundDto orderRefundDto)
        {
            _logger.LogInformation("RefundOrder triggered");

            try {
                await _orderService.RefundOrderAsync(orderRefundDto);
                return Ok();
            } catch ( Exception e ) {
                return BadRequest(e.Message);
            }
        }

        [HttpPut(Name = "UpdateStatusOrder")]
        public async Task<ActionResult> UpdateStatusOrderAsync(OrderStatusDto orderStatusDto)
        {
            _logger.LogInformation("UpdateStatusOrder triggered");

            try {
                await _orderService.UpdateStatusOrderAsync(orderStatusDto);
                return Ok();
            } catch ( Exception e ) {
                return BadRequest(e.Message);
            }
        }



    }
