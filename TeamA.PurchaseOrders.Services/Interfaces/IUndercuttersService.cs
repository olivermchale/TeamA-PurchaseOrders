using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IUndercuttersService : IOrdersService
    {
        Task<List<ExternalProductDto>> GetProducts();

        Task<ExternalProductDto> GetProduct(int id);
    }
}
