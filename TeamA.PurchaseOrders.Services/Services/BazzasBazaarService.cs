using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class BazzasBazaarService : IBazzasBazaarService
    {
        private StoreClient _storeClient;
        public BazzasBazaarService(StoreClient storeClient)
        {
            _storeClient = storeClient;
        }
        public async Task<ProductItemVm> GetProduct(int id)
        {
            await _storeClient.OpenAsync();
            var x = await _storeClient.GetProductByIdAsync(id);
            await _storeClient.CloseAsync();
            return new ProductItemVm
            {
                Product = new ProductDto
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    Ean = x.Ean,
                    ExpectedRestock = x.ExpectedRestock == null ? false : true,
                    Id = x.Id,
                    InStock = x.InStock,
                    Name = x.Name,
                    Price = x.PriceForOne
                },
                Source = "BazaasBazaar"
            };
        }
    }
}
