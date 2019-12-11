using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Repository.Interfaces;

namespace TeamA.PurchaseOrders.Repository.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private PurchaseOrdersDb _context;

        public OrdersRepository(PurchaseOrdersDb context)
        {
            _context = context;
        }

        public async Task<bool> CreateOrder(PurchaseOrderDto purchaseOrder)
        {
            try
            {
                await _context.PurchaseOrders.AddAsync(purchaseOrder);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                // todo : exception handling
            }
            return true;          
        }

        public async Task<OrderListVm> GetOrders()
        {
            var orders = await _context.PurchaseOrders
                                        .Select(b => new OrderListItemVm
                                        {
                                            Id = b.ID,
                                            OrderStatus = b.PurchaseStatus.Name,
                                            Price = b.ProductPrice,
                                            ProductName = b.ProductName,
                                            Quantity = b.Quantity
                                        }).ToListAsync();
            return new OrderListVm
            {
                Orders = orders
            };
        }
    }
}
