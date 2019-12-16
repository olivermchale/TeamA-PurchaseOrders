using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IBazzasBazaarService : IOrdersService
    {
        Task<ExternalProductDto> GetProduct(int id);

        Task<List<ExternalProductDto>> GetAllProducts();
    }
}
