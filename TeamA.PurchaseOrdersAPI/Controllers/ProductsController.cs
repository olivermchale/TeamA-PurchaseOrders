using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ProductsController : ControllerBase
    {
        private IProductsService _productsService;
        private readonly ILogger _logger;

        public ProductsController(IProductsService productsService, ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        [HttpGet("getProducts")]
        public async Task<IActionResult> GetAndSaveProducts()
        {
            _logger.LogInformation("Getting all products");
            var products = await _productsService.GetAndSaveProducts();
            if(products == false)
            {
                _logger.LogInformation("Unable to find products");
                return NotFound();
            }
            _logger.LogInformation("Successfully found products");
            return Ok(products);
        }

        [HttpGet("getsavedproducts")]
        public async Task<IActionResult> GetSavedProducts()
        {
            _logger.LogInformation("Getting all saved products");
            var products = await _productsService.GetProducts();
            if (products == null)
            {
                _logger.LogDebug("Failed to find any saved orders");
                return NotFound();
            }
            return Ok(products);
        }
        [HttpGet("getProductsByEan")]
        public async Task<IActionResult> GetProducts(string ean)
        {
            if(ean == null)
            {
                return BadRequest("No ean");
            }
            _logger.LogInformation("Finding all products with ean " + ean);
            var products = await _productsService.GetProductsByEan(ean);
            if(products == null)
            {
                _logger.LogDebug("Unable to find and products with ean: " + ean);
                return NotFound();
            }
            _logger.LogInformation("Successfully found all products"); 
            return Ok(products);
        }
    }
}