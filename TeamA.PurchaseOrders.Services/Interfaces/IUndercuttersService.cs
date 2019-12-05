using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IUndercuttersService
    {
        Task<List<ProductDto>> GetProducts();

        Task<ProductDto> GetProduct(int id);
    }
}
