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

        public async Task<Guid?> CreateOrder(PurchaseOrderDto purchaseOrder)
        {
            try
            {
                var dbEntity = await _context.PurchaseOrders.AddAsync(purchaseOrder);
                await _context.SaveChangesAsync();
                return dbEntity.Entity.ID;
            }
            catch(Exception e)
            {
                // todo : exception handling
            }
            return null;          
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

        public async Task<bool> UpdateOrderAsync(Guid? orderId, OrderCreatedDto createdOrder)
        {
            try
            {
                var order = await _context.PurchaseOrders.Where(a => a.ID == orderId).FirstOrDefaultAsync();
                if(order == null)
                {
                    return false;
                }
                order.ExternalID = createdOrder.Id;
                order.PurchaseStatus.Name = "Purchased";
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                //todo: exception handling
            }
            return false;
        }
    }
}
