using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class BazzasBazaarService : IBazzasBazaarService, IOrdersService
    {
        private StoreClient _storeClient;
        private readonly ILogger<BazzasBazaarService> _logger;
        private AsyncRetryPolicy _retryPolicy;

        public BazzasBazaarService(StoreClient storeClient, ILogger<BazzasBazaarService> logger)
        {
            _storeClient = storeClient;
            _logger = logger;
            _retryPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                 5,
                 retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 (exception, timeSpan, context) => {
                     var methodThatRaisedException = context["methodName"];
                     _logger.LogError("Exception in " + methodThatRaisedException + " " + exception + exception.StackTrace);
                 });
        }

        public async Task<OrderCreatedDto> CreateOrder(string accountName, string cardNumber, int productId, int quantity)
        {
            _logger.LogInformation("Create order - BazzasBazaarService");
           return await _retryPolicy.ExecuteAsync(async () =>
           {
               try
               {
                   _logger.LogInformation("Opening store client");
                   await _storeClient.OpenAsync();
                   _logger.LogInformation("Creating an order");
                   var order = await _storeClient.CreateOrderAsync(accountName, cardNumber, productId, quantity);
                   if (order == null)
                   {
                       _logger.LogError("Failed to create order");
                       return new OrderCreatedDto
                       {
                           Success = false
                       };
                   }
                   _logger.LogInformation("Successfully created order, closing store client");
                   await _storeClient.CloseAsync();
                   return new OrderCreatedDto
                   {
                       Id = order.Id,
                       AccountName = order.AccountName,
                       CardNumber = order.CardNumber,
                       ProductEan = order.ProductEan,
                       ProductId = order.ProductId,
                       ProductName = order.ProductName,
                       PurchasedOn = order.When,
                       Quantity = order.Quantity,
                       TotalPrice = order.TotalPrice,
                       Success = true
                   };
               }
               catch (Exception e)
               {
                   _logger.LogError("Exception when creating order" + e + e.StackTrace);
               }
               return new OrderCreatedDto
               {
                   Success = false
               };
           });
        }

        public async Task<ExternalProductDto> GetProduct(int id)
        {
            _logger.LogInformation("Getting product with id: " + id);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    _logger.LogInformation("Opening store client");
                    await _storeClient.OpenAsync();
                    var product = await _storeClient.GetProductByIdAsync(id);
                    await _storeClient.CloseAsync();
                    _logger.LogInformation("Got product, closing store client");
                    return new ExternalProductDto
                    {
                        CategoryId = product.CategoryId,
                        CategoryName = product.CategoryName,
                        Description = product.Description,
                        Ean = product.Ean,
                        ExpectedRestock = product.ExpectedRestock == null ? false : true,
                        Id = product.Id,
                        InStock = product.InStock,
                        Name = product.Name,
                        Price = product.PriceForOne,
                        Source = "BazzasBazaar"
                    };
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception when getting product with id: " + id + e + e.StackTrace);
                }
                return null;
            });
        }

        public async Task<List<ExternalProductDto>> GetAllProducts()
        {
            _logger.LogInformation("Getting all products");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var products = new List<ExternalProductDto>();
                    _logger.LogInformation("Opening store client");
                    await _storeClient.OpenAsync();
                    _logger.LogInformation("Getting all categories");
                    var categories = await _storeClient.GetAllCategoriesAsync();
                    foreach (var category in categories)
                    {
                        _logger.LogInformation("Getting products for category: " + category.Name);
                        var productArray = await _storeClient.GetFilteredProductsAsync(category.Id, category.Name, 0, 1000);
                        foreach (var product in productArray)
                        {
                            products.Add(new ExternalProductDto
                            {
                                CategoryId = product.CategoryId,
                                CategoryName = product.CategoryName,
                                Description = product.Description,
                                Ean = product.Ean,
                                ExpectedRestock = product.ExpectedRestock == null ? false : true,
                                Id = product.Id,
                                InStock = product.InStock,
                                Name = product.Name,
                                Price = product.PriceForOne,
                                Source = "BazzasBazaar"
                            });
                        }
                    }
                    _logger.LogInformation("Successfully got all products, closing client");
                    await _storeClient.CloseAsync();
                    return products;
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception when getting product" + e + e.StackTrace);
                }
                return null;
            });
        }
        
    }
}
