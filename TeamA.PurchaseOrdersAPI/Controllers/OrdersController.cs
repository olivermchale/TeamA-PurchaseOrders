using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class OrdersController : ControllerBase
    {

        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(PurchaseOrderDto orderInfo)
        {
            //todo: ask craig about decorators 
            return Ok(orderInfo);
        }
    }
}