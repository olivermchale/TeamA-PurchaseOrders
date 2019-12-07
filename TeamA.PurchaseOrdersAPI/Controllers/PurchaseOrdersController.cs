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
        private IProductsService _productsService;

        public PurchaseOrdersController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("getProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productsService.GetProducts();
            if(products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("getProduct")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var products = await _productsService.GetProduct(id);
            if(products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("baz")]
        public async Task<IActionResult> GetBaz(int id)
        {
            var product = await _productsService.GetBaz(id);
            return Ok(product);
        }
    }
}