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
        Task<bool> CreateOrder(PurchaseOrderDto purchaseOrder);

        Task<OrderListVm> GetOrders();
    }
}
