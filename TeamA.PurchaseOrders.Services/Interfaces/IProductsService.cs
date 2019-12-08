using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.ViewModels;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IProductsService
    {
        Task<List<ProductListVm>> GetProducts();

        Task<List<ProductItemVm>> GetProduct(int id);
    }
}
