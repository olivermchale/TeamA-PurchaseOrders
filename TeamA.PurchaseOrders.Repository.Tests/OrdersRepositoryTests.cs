using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Repository.Repositories;

namespace TeamA.PurchaseOrders.Repository.Tests
{
    public class OrdersRepositoryTests
    {
        private IOrdersRepository _ordersRepository;
        private IOrdersRepository _ordersRepositoryMockedContext;
        private Mock<PurchaseOrdersDb> _mockContext;
        private Mock<ILogger<OrdersRepository>> _mockLogger;
        private PurchaseStatusDto _stubPurchaseStatusDto;
        private PaymentInformationDto _stubPaymentInformationDto;
        private PurchaseOrderDto _stubPurchaseOrderDto;
        
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
                ID = Guid.NewGuid(),
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
            _mockLogger = new Mock<ILogger<OrdersRepository>>();
            _ordersRepository = new OrdersRepository(GetInMemoryContextWithSeedData(), _mockLogger.Object);
            _mockContext = GetMockedContext();
            _ordersRepositoryMockedContext = new OrdersRepository(_mockContext.Object, _mockLogger.Object);
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

        private Mock<PurchaseOrdersDb> GetMockedContext()
        {
            var context = new Mock<PurchaseOrdersDb>();
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
        public async Task GetOrders_NoOrders_Null()
        {
            // Arrange
           
            // Act
            
            // Assert
        }
    }
}