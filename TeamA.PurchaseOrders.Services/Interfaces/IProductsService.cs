using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IProductsService
    {
        Task<bool> GetAndSaveProducts();

        Task<List<ProductDto>> GetProducts();

        Task<List<ProductItemVm>> GetProduct(int id);

        Task<List<ProductDto>> GetProductsByEan(string ean);
    }
}
