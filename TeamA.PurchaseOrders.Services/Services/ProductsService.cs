using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IUndercuttersService _undercuttersService;
        private readonly IDodgyDealersService _dodgyDealersService;
        private readonly IBazzasBazaarService _bazzasBazaarService;
        private IProductsRepository _productsRepository;

        public ProductsService(IUndercuttersService undercuttersService, IDodgyDealersService dodgyDealersService, IBazzasBazaarService bazzasBazaarService, IProductsRepository productsRepository)
        {
            _undercuttersService = undercuttersService;
            _dodgyDealersService = dodgyDealersService;
            _bazzasBazaarService = bazzasBazaarService;
            _productsRepository = productsRepository;
        }

        public async Task<bool> GetAndSaveProducts()
        {
            var undercuttersProducts = await _undercuttersService.GetProducts();
            var dodgyDealersProducts = await _dodgyDealersService.GetProducts();
            var bazzasBazaarProducts = await _bazzasBazaarService.GetAllProducts();

            return await _productsRepository.SaveProducts(undercuttersProducts.Concat(dodgyDealersProducts).Concat(bazzasBazaarProducts));
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            return await _productsRepository.GetProducts();
        }

        public async Task<List<ProductItemVm>> GetProduct(int id)
        {
            var undercuttersProduct = await _undercuttersService.GetProduct(id);
            var dodgyDealersProduct = await _dodgyDealersService.GetProduct(id);
            var bazzasBazaarProduct = await _bazzasBazaarService.GetProduct(id);

            var undercuttersProductVm = new ProductItemVm
            {
                Product = undercuttersProduct,
                Source = "DodgyDealers"
            };

            var dodgyDealersProductVm = new ProductItemVm
            {
                Product = dodgyDealersProduct,
                Source = "DodgyDealers"
            };

            var bazaasBazaarProductVm = new ProductItemVm
            {
                Product = bazzasBazaarProduct,
                Source = "DodgyDealers"
            };


            return new List<ProductItemVm> { undercuttersProductVm, dodgyDealersProductVm, bazaasBazaarProductVm };
        }

        public async Task<List<ProductDto>> GetProductsByEan(string ean)
        {
            return await _productsRepository.GetProductsByEan(ean);
        }
    }
}
