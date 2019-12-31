using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Repository.Repositories;

namespace TeamA.PurchaseOrders.Repository.Tests
{
    public class OrdersRepositoryTests
    {
        private IOrdersRepository _ordersRepository;
        private Mock<ILogger<OrdersRepository>> _mockLogger;
        private PurchaseStatusDto _stubPurchaseStatusDto;
        private PaymentInformationDto _stubPaymentInformationDto;
        private PurchaseOrderDto _stubPurchaseOrderDto;
        private OrderCreatedDto _stubOrderCreatedDto;
        
        [SetUp]
        public void Setup()
        {
            _stubPurchaseStatusDto = new PurchaseStatusDto
            {
                Id = Guid.NewGuid(),
                Name = "Ordered"
            };
            _stubPaymentInformationDto = new PaymentInformationDto
            {
                CardCVC = "121",
                CardExpiry = new DateTime(),
                CardName = "Oliver Test",
                CardNumber = "1234567890123456",
                ID = Guid.NewGuid()
            };
            _stubPurchaseOrderDto = new PurchaseOrderDto
            {
                Address = "Test Drive",
                ExternalID = 1,
                ID = Guid.Parse("d61a78a9-b6ad-4430-91ea-0c8d5227b6aa"),
                IsDeleted = false,
                PaymentInformation = _stubPaymentInformationDto,
                PaymentInformationID = _stubPaymentInformationDto.ID,
                Postcode = "T35T DR1",
                ProductID = Guid.NewGuid(),
                ProductName = "Testy",
                ProductPrice = 10.50,
                PurchasedBy = Guid.NewGuid(),
                PurchasedOn = new DateTime(),
                PurchaseStatus = _stubPurchaseStatusDto,
                Quantity = 1,
                Source = "Undercutters",
                StatusID = _stubPurchaseStatusDto.Id
            };
            _stubOrderCreatedDto = new OrderCreatedDto
            {
                AccountName = "Testy1",
                CardNumber = "10490249204920492049",
                Id = 1,
                ProductEan = "1 2 4 2 3",
                ProductId = 1,
                ProductName = "Product1",
                PurchasedOn = new DateTime(),
                Quantity = 5,
                Success = true,
                TotalPrice = 10
            };
            _mockLogger = new Mock<ILogger<OrdersRepository>>();
            _ordersRepository = new OrdersRepository(GetInMemoryContextWithSeedData(), _mockLogger.Object);
        }

        private PurchaseOrdersDb GetInMemoryContextWithSeedData()
        {
            var options = new DbContextOptionsBuilder<PurchaseOrdersDb>()
                                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                .Options;
            var context = new PurchaseOrdersDb(options);
            context.PurchaseStatus.Add(_stubPurchaseStatusDto);
            context.PaymentInformation.Add(_stubPaymentInformationDto);
            context.PurchaseOrders.Add(_stubPurchaseOrderDto);

            context.SaveChanges();

            return context;
        }

        [Test]
        public async Task GetOrders_Success()
        {
            // Act
            var orders = await _ordersRepository.GetOrders();

            // Assert
            Assert.AreEqual(orders.Orders.First().Id, _stubPurchaseOrderDto.ID);
            Assert.AreEqual(orders.Orders.First().OrderStatus, _stubPurchaseOrderDto.PurchaseStatus.Name);
            Assert.AreEqual(orders.Orders.First().Price, _stubPurchaseOrderDto.ProductPrice);
            Assert.AreEqual(orders.Orders.First().ProductName, _stubPurchaseOrderDto.ProductName);
            Assert.AreEqual(orders.Orders.First().Price, _stubPurchaseOrderDto.ProductPrice);
        }

        [Test]
        public async Task CreateOrder_Success()
        {
            // Act
            var order = await _ordersRepository.CreateOrder(_stubPurchaseOrderDto);

            // Assert
            Assert.IsNotNull(order);
            Assert.IsInstanceOf<Guid>(order);
        }

        [Test]
        public async Task GetOrder_Valid_Success()
        {
            var order = await _ordersRepository.GetOrder(Guid.Parse("d61a78a9-b6ad-4430-91ea-0c8d5227b6aa"));

            Assert.IsNotNull(order);
            Assert.IsInstanceOf<OrderDetailVm>(order);
            Assert.AreEqual(order.Address, _stubPurchaseOrderDto.Address);
            Assert.AreEqual(order.CardholderName, _stubPurchaseOrderDto.PaymentInformation.CardName);
            Assert.AreEqual(order.Id, _stubPurchaseOrderDto.ID);
            Assert.AreEqual(order.OrderPrice, _stubPurchaseOrderDto.ProductPrice);
            Assert.AreEqual(order.Postcode, _stubPurchaseOrderDto.Postcode);
            Assert.AreEqual(order.ProductId, _stubPurchaseOrderDto.ProductID);
            Assert.AreEqual(order.ProductName, _stubPurchaseOrderDto.ProductName);
            Assert.AreEqual(order.ProductPrice, _stubPurchaseOrderDto.ProductPrice);
            Assert.AreEqual(order.PurchasedOn, _stubPurchaseOrderDto.PurchasedOn);
            Assert.AreEqual(order.Quantity, _stubPurchaseOrderDto.Quantity);
            Assert.AreEqual(order.Source, _stubPurchaseOrderDto.Source);
        }

        [Test]
        public async Task GetOrder_DoesntExist_Null()
        {
            var order = await _ordersRepository.GetOrder(Guid.NewGuid());

            Assert.IsNull(order);
        }

        [Test]
        public async Task UpdateOrder_Exists_Valid_True()
        {
            var success = await _ordersRepository.UpdateOrderAsync(Guid.Parse("d61a78a9-b6ad-4430-91ea-0c8d5227b6aa"), _stubOrderCreatedDto, "Complete");

            Assert.IsNotNull(success);
            Assert.IsTrue(success);
        }

        [Test]
        public async Task UpdateOrder_NoOrderExists_Fails_False()
        {
            var success = await _ordersRepository.UpdateOrderAsync(Guid.NewGuid(), _stubOrderCreatedDto, "Complete");

            Assert.IsNotNull(success);
            Assert.IsFalse(success);
        }
    }
}