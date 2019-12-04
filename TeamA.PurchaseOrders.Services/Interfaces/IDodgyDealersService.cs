using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IDodgyDealersService
    {
        Task<ProductArrayDto> GetProducts();
    }
}
