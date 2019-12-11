using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Repository.Interfaces;

namespace TeamA.PurchaseOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class OrdersController : ControllerBase
    {
        private IOrdersRepository _ordersRepository;

        public OrdersController(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;       
        }
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(PurchaseOrderDto orderInfo)
        {
            var success = await _ordersRepository.CreateOrder(orderInfo);
            return Ok(success);
        }

        [HttpGet("getOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _ordersRepository.GetOrders();
            return Ok(orders);
        }
    }
}