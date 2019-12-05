using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IUndercuttersService _undercuttersService;
        private readonly IDodgyDealersService _dodgyDealersService;

        public PurchaseOrdersController(IUndercuttersService undercuttersService, IDodgyDealersService dodgyDealersService)
        {
            _undercuttersService = undercuttersService;
            _dodgyDealersService = dodgyDealersService;
        }

        [HttpGet("getPrices")]
        public async Task<IActionResult> GetPrices()
        {
            var undercuttersPrices = await _undercuttersService.GetProducts();
            var dodgyDealersPrices = await _dodgyDealersService.GetProducts();

            return Ok(undercuttersPrices.Concat(dodgyDealersPrices));
        }

        [HttpGet("GetUndercuttersPrices")]
        public async Task<IActionResult> GetUndercuttersPrices()
        {
            var undercuttersPrices = await _undercuttersService.GetProducts();
            return Ok(undercuttersPrices);
        }


    }
}