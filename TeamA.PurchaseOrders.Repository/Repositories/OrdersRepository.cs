using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
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
        private readonly ILogger<OrdersRepository> _logger;
        private AsyncRetryPolicy _retryPolicy;

        public OrdersRepository(PurchaseOrdersDb dbContext, ILogger<OrdersRepository> logger)
        {
            _context = dbContext;
            _logger = logger;
            _retryPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                 5,
                 retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 (exception, timeSpan, context) =>
                 {
                     var methodThatRaisedException = context["methodName"];
                     _logger.LogError("Exception in " + methodThatRaisedException + " " + exception + exception.StackTrace);
                 });
        }

        public async Task<Guid> CreateOrder(PurchaseOrderDto purchaseOrder)
        {
            _logger.LogInformation("Creating an order in local database");
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var dbEntity = await _context.PurchaseOrders.AddAsync(purchaseOrder);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully created an order in local database");
                    return dbEntity.Entity.ID;
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception when trying to create a new purchase order:" + purchaseOrder + e + e.StackTrace);
                }
                return Guid.Empty;
            });
        }

        public async Task<OrderListVm> GetOrders()
        {
            _logger.LogInformation("Getting all orders in local database");
            return await _retryPolicy.ExecuteAsync(async () =>
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
                    _logger.LogInformation("Successfull got all orders in local database");
                    return new OrderListVm
                    {
                        Orders = orders
                    };
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception when getting orders from database:" + e + e.StackTrace);
                }
                return null;
            });


        }

        public async Task<OrderDetailVm> GetOrder(Guid? id)
        {
            _logger.LogInformation("Getting order in database with id: " + id);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var order = await _context.PurchaseOrders.Where(o => o.ID == id)
                        .Include(p => p.PaymentInformation)
                        .Include(p => p.PurchaseStatus)
                        .FirstOrDefaultAsync();
                    if (order != null)
                    {
                        _logger.LogInformation("Successfully retrieved order from database with id: " + id);
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
                catch (Exception e)
                {
                    _logger.LogError("Exception when trying to retrieve order with id: " + id + e + e.StackTrace);
                }
                return null;
            });

        }

        public async Task<bool> UpdateOrderAsync(Guid? orderId, OrderCreatedDto createdOrder, string status)
        {
            _logger.LogInformation("Updating order with id: " + orderId);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var order = await _context.PurchaseOrders.Where(a => a.ID == orderId).FirstOrDefaultAsync();
                    if (order == null)
                    {
                        return false;
                    }
                    if (createdOrder != null)
                    {
                        order.ExternalID = createdOrder.Id;
                    }
                    order.PurchaseStatus.Name = status;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated order with id: " + orderId);
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception when trying to update order with id: " + orderId + e + e.StackTrace);
                }
                return false;
            });
        }
    }
}
