using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<OrderCreatedDto> CreateOrder(string accountName, string cardNumber, int productId, int quantity);
    }
}
