using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private OrdersFactory _ordersFactory;

        public OrdersController(IOrdersRepository ordersRepository, OrdersFactory ordersFactory)
        {
            _ordersRepository = ordersRepository;
            _ordersFactory = ordersFactory;
        }
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(PurchaseOrderDto orderInfo)
        {
            var orderId = await _ordersRepository.CreateOrder(orderInfo);
            if(orderId != null)
            {
                var service = _ordersFactory.Create(orderInfo.Source);
                var order = await service.CreateOrder(orderInfo.PaymentInformation.CardName, orderInfo.PaymentInformation.CardNumber, orderInfo.ProductID, orderInfo.Quantity);
                if(order != null)
                {
                    var success = await _ordersRepository.UpdateOrderAsync(orderId, order);
                    return Ok(success);
                };            
            }
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        }

        [HttpGet("getOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _ordersRepository.GetOrders();
            return Ok(orders);
        }
    }
}