using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ProductsController : ControllerBase
    {
        private IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("getProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productsService.GetAndSaveProducts();
            if(products == false)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("getsavedproducts")]
        public async Task<IActionResult> GetSavedProducts()
        {
            var products = await _productsService.GetProducts();
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }
        [HttpGet("getProductsByEan")]
        public async Task<IActionResult> GetProducts(string ean)
        {
            var products = await _productsService.GetProductsByEan(ean);
            if(products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }
    }
}