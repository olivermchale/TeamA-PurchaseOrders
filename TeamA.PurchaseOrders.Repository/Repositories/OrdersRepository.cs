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
            try
            {
                var orders = await _context.PurchaseOrders
                            .Select(b => new OrderListItemVm
                            {
                                Id = b.ID,
                                OrderStatus = b.PurchaseStatus.Name,
                                Price = b.ProductPrice * b.Quantity,
                                ProductName = b.ProductName,
                                Quantity = b.Quantity
                            }).ToListAsync();
                orders.Reverse();
                return new OrderListVm
                {
                    Orders = orders
                };
            }
            catch(Exception e)
            {
                // todo: exception handling 
            }
            return null;

        }

        public async Task<OrderDetailVm> GetOrder(Guid id)
        {
            try
            {
                //todo: create vm and return (i.e. get last 4 digits of card etc)
                var order = await _context.PurchaseOrders.Where(o => o.ID == id)
                    .Include(p => p.PaymentInformation)
                    .Include(p => p.PurchaseStatus)
                    .FirstOrDefaultAsync();
                if (order != null)
                {
                    return new OrderDetailVm
                    {
                        Address = order.Address,
                        CardholderName = order.PaymentInformation.CardName,
                        Last4Digits = order.PaymentInformation.CardNumber.Substring(order.PaymentInformation.CardNumber.Length - 4),
                        Id = order.ID,
                        OrderPrice = order.ProductPrice * order.Quantity,
                        PurchasedOn = order.PurchasedOn,
                        Postcode = order.Postcode,
                        ProductPrice = order.ProductPrice,
                        ProductId = order.ProductID,
                        ProductName = order.ProductName,
                        PurchaseStatus = order.PurchaseStatus.Name,
                        Quantity = order.Quantity,
                        Source = order.Source
                    };
                }
            }
            catch(Exception e)
            {
                // todo: exception handling
            }
            return null;
        }

        public async Task<bool> UpdateOrderAsync(Guid? orderId, OrderCreatedDto createdOrder, string status)
        {
            try
            {
                var order = await _context.PurchaseOrders.Where(a => a.ID == orderId).FirstOrDefaultAsync();
                if(order == null)
                {
                    return false;
                }
                if(createdOrder != null)
                {
                    order.ExternalID = createdOrder.Id;
                }
                order.PurchaseStatus.Name = status;
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
