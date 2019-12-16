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

        public BazzasBazaarService()
        {
        }

        public BazzasBazaarService(StoreClient storeClient)
        {
            _storeClient = storeClient;
        }

        public Task<OrderCreatedDto> CreateOrder(string accountName, string cardNumber, int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task<ExternalProductDto> GetProduct(int id)
        {
            await _storeClient.OpenAsync();
            var x = await _storeClient.GetProductByIdAsync(id);
            await _storeClient.CloseAsync();
            return new ExternalProductDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Description = x.Description,
                Ean = x.Ean,
                ExpectedRestock = x.ExpectedRestock == null ? false : true,
                Id = x.Id,
                InStock = x.InStock,
                Name = x.Name,
                Price = x.PriceForOne,
                Source = "BazzasBazaar"
            };
        }

        public async Task<List<ExternalProductDto>> GetAllProducts()
        {
            var products = new List<ExternalProductDto>();
            await _storeClient.OpenAsync();
            var categories = await _storeClient.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                var productArray = await _storeClient.GetFilteredProductsAsync(category.Id, category.Name, 0, 1000);
                foreach(var product in productArray)
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
                        Price = product.PriceForOne
                    });
                }
            }
            return products;
        }
        
    }
}
