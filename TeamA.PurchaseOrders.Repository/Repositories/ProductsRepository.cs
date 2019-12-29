using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Repository.Interfaces;

namespace TeamA.PurchaseOrders.Repository.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private PurchaseOrdersDb _context;
        private readonly ILogger<ProductsRepository> _logger;
        private AsyncRetryPolicy _retryPolicy;

        public ProductsRepository(PurchaseOrdersDb dbContext, ILogger<ProductsRepository> logger)
        {
            _context = dbContext;
            _logger = logger;
            _retryPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                 5,
                 retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 (exception, timeSpan, context) =>
                 {
                     var methodThatRaisedException = context["methodName"];
                     _logger.LogError("Exception in " + methodThatRaisedException + " " + exception + exception.StackTrace);
                 });
        }
        public async Task<List<ProductDto>> GetProducts()
        {
            _logger.LogInformation("Getting all products from database!");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var products = await _context.Products.Select(p => new ProductDto
                    {
                        CategoryId = p.CategoryId,
                        CategoryName = p.CategoryName,
                        Description = p.Description,
                        Ean = p.Ean,
                        ExpectedRestock = p.ExpectedRestock,
                        Id = p.Id,
                        InStock = p.InStock,
                        Name = p.Name,
                        Price = p.Price,
                        Source = p.Source,
                        ExternalId = p.ExternalId
                    }).ToListAsync();
                    _logger.LogInformation("Successfully retrieved products from database");
                    return products;
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception when getting products from database: " + e + e.StackTrace);
                }
                return null;
            });
        }

        public async Task<bool> SaveProducts(IEnumerable<ExternalProductDto> products)
        {
            _logger.LogInformation("Attempting to save products to database");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    foreach (var product in products)
                    {
                        if (!_context.Products.Any(p => p.Ean == product.Ean && p.Source == product.Source))
                        {
                            // Product doesnt exist, add it
                            var item = new ProductDto
                            {
                                Id = Guid.NewGuid(),
                                InStock = product.InStock,
                                CategoryId = product.CategoryId,
                                CategoryName = product.CategoryName,
                                Description = product.Description,
                                Ean = product.Ean,
                                ExpectedRestock = product.ExpectedRestock,
                                ExternalId = product.Id,
                                Name = product.Name,
                                Price = product.Price,
                                Source = product.Source
                            };
                            await _context.Products.AddAsync(item);
                        }
                        else
                        {
                            // Product exists, lets update it
                            var existingProduct = await _context.Products.Where(s => s.Ean == product.Ean && s.Source == product.Source).FirstOrDefaultAsync();
                            existingProduct.InStock = product.InStock;
                            existingProduct.CategoryId = product.CategoryId;
                            existingProduct.CategoryName = product.CategoryName;
                            existingProduct.Description = product.Description;
                            existingProduct.Ean = product.Ean;
                            existingProduct.ExpectedRestock = product.ExpectedRestock;
                            existingProduct.ExternalId = product.Id;
                            existingProduct.Name = product.Name;
                            existingProduct.Price = product.Price;
                            existingProduct.Source = product.Source;

                            _context.Update(existingProduct);
                        }
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully saved products to database");
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError("Failed to save products to database: " + products + e + e.StackTrace);
                }
                return false;
            });
        }

        public async Task<List<ProductDto>> GetProductsByEan(string ean)
        {
            _logger.LogInformation("Getting products from database with ean: " + ean);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                if (ean == null)
                {
                    _logger.LogDebug("No ean given");
                    return null;
                }
                try
                {
                    var products = await _context.Products.Where(p => p.Ean == ean).ToListAsync();
                    _logger.LogInformation("Successfully retrieved products from database");
                    return products;
                }
                catch (Exception e)
                {
                    _logger.LogError("Failed to get products from database with ean: " + ean + e + e.StackTrace);
                }
                return null;
            });
        }
    }
}
