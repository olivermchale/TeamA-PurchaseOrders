using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Services.Factories;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class OrdersController : ControllerBase
    {
        private IOrdersRepository _ordersRepository;
        private IOrdersFactory _ordersFactory;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrdersRepository ordersRepository, IOrdersFactory ordersFactory, ILogger<OrdersController> logger)
        {
            _ordersRepository = ordersRepository;
            _ordersFactory = ordersFactory;
            _logger = logger;
        }
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(PurchaseOrderDto orderInfo)
        {
            _logger.LogInformation("Creating a new order!");
            var orderId = await _ordersRepository.CreateOrder(orderInfo);
            if(orderId != null)
            {
                var service = _ordersFactory.Create(orderInfo.Source);
                var order = await service.CreateOrder(orderInfo.PaymentInformation.CardName, orderInfo.PaymentInformation.CardNumber, orderInfo.ExternalID, orderInfo.Quantity);
                if(order.Success == false)
                {
                    _logger.LogInformation("Failed to create an order due to insufficient stock");
                    await _ordersRepository.UpdateOrderAsync(orderId, null, "Insucfficient Stock");
                    return BadRequest("Insufficient Stock");
                }
                if(order != null)
                {
                    _logger.LogInformation("Successfully created a new order - " + orderId);
                    var success = await _ordersRepository.UpdateOrderAsync(orderId, order, "Complete");
                    return Ok(success);
                };
                await _ordersRepository.UpdateOrderAsync(orderId, null, "Failed");
            }
            _logger.LogError("Failed to create a new order - reason unknown");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        }

        [HttpGet("getOrders")]
        public async Task<IActionResult> GetOrders()
        {
            _logger.LogInformation("Getting all orders");
            var orders = await _ordersRepository.GetOrders();
            if(orders == null)
            {
                _logger.LogDebug("No orders or failed to get orders");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            _logger.LogInformation("Successfully got orders");
            return Ok(orders);
        }

        [HttpGet("getOrder")]
        public async Task<IActionResult> GetOrder(Guid? id)
        {
            if(id == null)
            {
                return BadRequest("No Id");
            }
            _logger.LogInformation("Getting order: " + id);
            var order = await _ordersRepository.GetOrder(id);
            if (order != null)
            {
                _logger.LogInformation("Successfully got order: " + id);
                return Ok(order);
            }
            _logger.LogError("Failed to find order with id: " + id);
            return NotFound();
        }
    }
}