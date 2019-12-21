using Polly;
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
        private Policy _retryPolicy;

        public BazzasBazaarService(StoreClient storeClient)
        {
            _storeClient = storeClient;
            _retryPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetry(
                 5,
                 retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 (exception, timeSpan, context) => {
                     var methodThatRaisedException = context["methodName"];
                 });
        }

        public async Task<OrderCreatedDto> CreateOrder(string accountName, string cardNumber, int productId, int quantity)
        {
           return await _retryPolicy.Execute(async () =>
           {
               try
               {
                   await _storeClient.OpenAsync();
                   var order = await _storeClient.CreateOrderAsync(accountName, cardNumber, productId, quantity);
                   if (order == null)
                   {
                       return new OrderCreatedDto
                       {
                           Success = false
                       };
                   }
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
                   // todo: exception handling
               }
               return new OrderCreatedDto
               {
                   Success = false
               };
           });

        }

        public async Task<ExternalProductDto> GetProduct(int id)
        {
            return await _retryPolicy.Execute(async () =>
            {
                try
                {
                    await _storeClient.OpenAsync();
                    var product = await _storeClient.GetProductByIdAsync(id);
                    await _storeClient.CloseAsync();
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
                    // todo: exception handling
                }
                return null;
            });
        }

        public async Task<List<ExternalProductDto>> GetAllProducts()
        {
            return await _retryPolicy.Execute(async () =>
            {
                try
                {
                    var products = new List<ExternalProductDto>();
                    await _storeClient.OpenAsync();
                    var categories = await _storeClient.GetAllCategoriesAsync();
                    foreach (var category in categories)
                    {
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
                    return products;
                }
                catch (Exception e)
                {
                    // todo: exception handling
                }
                return null;
            });
        }
        
    }
}
