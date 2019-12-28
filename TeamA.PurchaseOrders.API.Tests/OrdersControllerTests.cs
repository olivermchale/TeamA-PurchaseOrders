using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Repository.Repositories;
using TeamA.PurchaseOrders.Services.Factories;
using TeamA.PurchaseOrders.Services.Interfaces;
using TeamA.PurchaseOrdersAPI.Controllers;

namespace TeamA.PurchaseOrders.API.Tests
{
    public class OrdersControllerTests
    {
        private OrdersController _ordersController;
        private Mock<IOrdersRepository> _mockOrdersRepository;
        private Mock<IOrdersFactory> _mockOrdersFactory;
        private Mock<ILogger<OrdersController>> _mockLogger;
        private Mock<IOrdersService> _mockOrdersService;
        private PurchaseOrderDto _stubPurchaseOrderDto;
        private OrderCreatedDto _stubOrderCreatedDto;
        private OrderCreatedDto _stubOrderFailedToCreateDto;
        private OrderListItemVm _stubOrderListItem;
        private List<OrderListItemVm> _stubOrderListList;
        private OrderListVm _stubOrderList;
        private OrderDetailVm _stubOrderDetailVm;

        [SetUp]
        public void Setup()
        {
            _mockOrdersRepository = new Mock<IOrdersRepository>();
            _mockOrdersFactory = new Mock<IOrdersFactory>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _mockOrdersService = new Mock<IOrdersService>();
            _ordersController = new OrdersController(_mockOrdersRepository.Object, _mockOrdersFactory.Object, _mockLogger.Object);
            _stubPurchaseOrderDto = new PurchaseOrderDto
            {
                Address = "MockAddress",
                ExternalID = 1,
                ID = Guid.NewGuid(),
                IsDeleted = false,
                PaymentInformation = new PaymentInformationDto
                {
                    ID = Guid.NewGuid(),
                    CardCVC = "121",
                    CardExpiry = new DateTime(),
                    CardName = "MockOli",
                    CardNumber = "1234567890123456"
                },
                Postcode = "T35T TST",
                ProductID = Guid.NewGuid(),
                ProductName = "Testy Producty",
                ProductPrice = 50.55,
                PurchasedBy = Guid.NewGuid(),
                PurchasedOn = new DateTime(),
                PurchaseStatus = new PurchaseStatusDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Ordered"
                },
                Quantity = 2,
                Source = "Undercutters",
            };
            _stubOrderCreatedDto = new OrderCreatedDto
            {
                Success = true,
            };
            _stubOrderFailedToCreateDto = new OrderCreatedDto
            {
                Success = false
            };
            _stubOrderListItem = new OrderListItemVm
            {
                Id = Guid.NewGuid(),
                OrderStatus = "Ordered",
                Price = 10.100,
                ProductName = "Test Product",
                Quantity = 5
            };
            _stubOrderListList = new List<OrderListItemVm>();
            _stubOrderListList.Add(_stubOrderListItem);
            _stubOrderList = new OrderListVm
            {
                Orders = _stubOrderListList
            };
            _stubOrderDetailVm = new OrderDetailVm
            {
                OrderPrice = 10,
                Address = "Test",
                CardholderName = "Oli",
                Id = Guid.NewGuid(),
                Last4Digits = "123",
                Postcode = "TSE3 231",
                ProductId = Guid.NewGuid(),
                ProductName = "Test",
                ProductPrice = 5.99,
                PurchasedOn = new DateTime(),
                PurchaseStatus = "Purchased",
                Quantity = 5,
                Source = "Undercutters"
            };
        }

        [Test]
        public async Task CreatePurchaseOrder_ValidOrder_Success()
        {
            // Arrange
            _mockOrdersRepository.Setup(s => s.CreateOrder(It.IsAny<PurchaseOrderDto>()))
                .ReturnsAsync(Guid.NewGuid());
            _mockOrdersFactory.Setup(s => s.Create(It.IsAny<string>())).Returns(_mockOrdersService.Object);
            _mockOrdersService.Setup(s => s.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_stubOrderCreatedDto);
            _mockOrdersRepository.Setup(m => m.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<OrderCreatedDto>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _ordersController.CreateOrder(_stubPurchaseOrderDto) as OkObjectResult;

            // Assert
            Assert.AreEqual(result.StatusCode, 200);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value, true);
            _mockOrdersRepository.Verify(m => m.CreateOrder(It.IsAny<PurchaseOrderDto>()), Times.Once);
            _mockOrdersFactory.Verify(m => m.Create(It.IsAny<string>()), Times.Once);
            _mockOrdersService.Verify(m => m.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockOrdersRepository.Verify(m => m.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<OrderCreatedDto>(), It.IsAny<string>()), Times.Once);

        }

        [Test]
        public async Task CreatePurchaseOrder_FailToSaveToDatabase_Fails()
        {
            // Arrange
            _mockOrdersRepository.Setup(s => s.CreateOrder(It.IsAny<PurchaseOrderDto>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _ordersController.CreateOrder(_stubPurchaseOrderDto) as StatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
            _mockOrdersRepository.Verify(m => m.CreateOrder(It.IsAny<PurchaseOrderDto>()), Times.Once);
        }

        [Test]
        public async Task CreatePurchaseOrder_CreateOrderFails_BadRequest_InsufficientStock()
        {
            // Arrange
            _mockOrdersRepository.Setup(s => s.CreateOrder(It.IsAny<PurchaseOrderDto>()))
                .ReturnsAsync(Guid.NewGuid());
            _mockOrdersFactory.Setup(s => s.Create(It.IsAny<string>())).Returns(_mockOrdersService.Object);
            _mockOrdersService.Setup(s => s.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_stubOrderFailedToCreateDto);
            _mockOrdersRepository.Setup(m => m.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<OrderCreatedDto>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _ordersController.CreateOrder(_stubPurchaseOrderDto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "Insufficient Stock");
            _mockOrdersRepository.Verify(m => m.CreateOrder(It.IsAny<PurchaseOrderDto>()), Times.Once);
            _mockOrdersFactory.Verify(m => m.Create(It.IsAny<string>()), Times.Once);
            _mockOrdersService.Verify(m => m.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockOrdersRepository.Verify(m => m.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<OrderCreatedDto>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetOrders_Success()
        {
            // Arrange
            _mockOrdersRepository.Setup(m => m.GetOrders())
                .ReturnsAsync(_stubOrderList);

            // Act
            var result = await _ordersController.GetOrders() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, _stubOrderList);
            _mockOrdersRepository.Verify(m => m.GetOrders(), Times.Once);
        }

        [Test]
        public async Task GetOrders_Null_Fails()
        {
            // Arrange
            _mockOrdersRepository.Setup(m => m.GetOrders())
                .ReturnsAsync(() => null);

            // Act
            var result = await _ordersController.GetOrders() as StatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
            _mockOrdersRepository.Verify(m => m.GetOrders(), Times.Once);
        }

        [Test]
        public async Task GetOrder_Success()
        {
            // Arrange
            _mockOrdersRepository.Setup(m => m.GetOrder(It.IsAny<Guid>()))
                .ReturnsAsync(_stubOrderDetailVm);

            // Act
            var result = await _ordersController.GetOrder(It.IsAny<Guid>()) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, _stubOrderDetailVm);
            _mockOrdersRepository.Verify(m => m.GetOrder(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task GetOrder_NoId_BadRequest()
        {
            // Arrange

            // Act
            var result = await _ordersController.GetOrder(null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "No Id");
            _mockOrdersRepository.Verify(m => m.GetOrder(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task GetOrder_NoOrderFound_NotFound()
        {
            // Arrange
            _mockOrdersRepository.Setup(m => m.GetOrder(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _ordersController.GetOrder(It.IsAny<Guid>()) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
             Assert.AreEqual(result.StatusCode, 404);
            _mockOrdersRepository.Verify(m => m.GetOrder(It.IsAny<Guid>()), Times.Once);

        }
    }
}