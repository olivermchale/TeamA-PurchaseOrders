using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;

namespace TeamA.PurchaseOrders.Repository.Interfaces
{
    public interface IOrdersRepository
    {
        Task<Guid?> CreateOrder(PurchaseOrderDto purchaseOrder);

        Task<OrderListVm> GetOrders();
        Task <bool> UpdateOrderAsync(Guid? orderId, OrderCreatedDto createdOrder, string status);
    }
}
