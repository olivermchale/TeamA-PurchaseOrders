using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IUndercuttersService _undercuttersService;
        private readonly IDodgyDealersService _dodgyDealersService;

        public ProductsService(IUndercuttersService undercuttersService, IDodgyDealersService dodgyDealersService)
        {
            _undercuttersService = undercuttersService;
            _dodgyDealersService = dodgyDealersService;
        }

        public async Task<List<ProductListVm>> GetProducts()
        {
            var undercuttersProducts = await _undercuttersService.GetProducts();
            var dodgyDealersProducts = await _dodgyDealersService.GetProducts();

            var undercuttersList = new ProductListVm
            {
                Source = "Undercutters",
                Products = undercuttersProducts
            };

            var dodgyDealersList = new ProductListVm
            {
                Source = "Dodgy Dealers",
                Products = dodgyDealersProducts
            };

            return new List<ProductListVm> { undercuttersList, dodgyDealersList };
        }

        public async Task<List<ProductItemVm>> GetProduct(int id)
        {
            var undercuttersProduct = await _undercuttersService.GetProduct(id);
            var dodgyDealersProduct = await _dodgyDealersService.GetProduct(id);

            var undercuttersProductVm = new ProductItemVm
            {
                Product = undercuttersProduct,
                Source = "Undercutters"
            };

            var dodgyDealersProductVm = new ProductItemVm
            {
                Product = dodgyDealersProduct,
                Source = "Dodgy Dealers"
            };

            return new List<ProductItemVm> { undercuttersProductVm, dodgyDealersProductVm };
        }
    }
}
