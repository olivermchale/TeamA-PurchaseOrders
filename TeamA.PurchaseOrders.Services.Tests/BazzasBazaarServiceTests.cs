using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Services;

namespace TeamA.PurchaseOrders.Services.Tests
{
    public class BazzasBazaarServiceTests
    {
        private Mock<StoreClient> _mockStoreClient;
        private Mock<ILogger<BazzasBazaarService>> _mockLogger;
        private BazzasBazaarService _bazzasBazaarService;
        private BazzasBazaar.Service.Category[] _stubCategories = new BazzasBazaar.Service.Category[1];
        private BazzasBazaar.Service.Product[] _stubProducts = new BazzasBazaar.Service.Product[1];

        [SetUp]
        public void Setup()
        {
            _mockStoreClient = new Mock<StoreClient>();
            _mockLogger = new Mock<ILogger<BazzasBazaarService>>();
            _bazzasBazaarService = new BazzasBazaarService(_mockStoreClient.Object, _mockLogger.Object);
            _stubCategories[0] = new BazzasBazaar.Service.Category
            {
                // todo: check available product count before getting products
                AvailableProductCount = 5,
                Description = "test",
                Id = 2,
                Name = "test category"
            };
            _stubProducts[0] = new BazzasBazaar.Service.Product
            {
                CategoryId = 1,
                CategoryName = "test category",
                Description = "TestDesc",
                Ean = "134-094231",
                ExpectedRestock = new DateTime(),
                Id = 2,
                InStock = true,
                Name = "Oli",
                PriceForOne = 100,
                PriceForTen = 1000
            };
        }

        [Test]
        public async Task CreateOrder_Success()
        {
            // Arrange
            _mockStoreClient.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new BazzasBazaar.Service.Order
                {
                    AccountName = "Test",
                    CardNumber = "294024902949820",
                    Id = 1,
                    ProductEan = "121212",
                    ProductId = 1,
                    ProductName = "Product",
                    Quantity = 3,
                    TotalPrice = 10.99,
                    When = new DateTime(),
                });

            // Act
            var result = await _bazzasBazaarService.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OrderCreatedDto>(result);
            _mockStoreClient.Verify(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task CreateOrder_FailToCreate_Failure()
        {
            // Arrange
            _mockStoreClient.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => null);

            // Act
            var order = await _bazzasBazaarService.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(order);
            Assert.IsInstanceOf<OrderCreatedDto>(order);
            Assert.IsFalse(order.Success);
            _mockStoreClient.Verify(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GetProductById_Success()
        {
            // Arrange
            _mockStoreClient.Setup(m => m.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new BazzasBazaar.Service.Product
                {
                    CategoryId = 1,
                    CategoryName = "Test",
                    Description = "TestD",
                    Ean = "134-0941",
                    ExpectedRestock = new DateTime(),
                    Id = 1,
                    InStock = true,
                    Name = "Oli",
                    PriceForOne = 10,
                    PriceForTen = 100
                });

            // Act
            var product = await _bazzasBazaarService.GetProduct(It.IsAny<int>());
            
            // Assert
            Assert.IsNotNull(product);
            Assert.IsInstanceOf<ExternalProductDto>(product);
            Assert.AreEqual(1, product.Id);
            _mockStoreClient.Verify(m => m.GetProductByIdAsync(It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GetProductById_Throws_ReturnsNull()
        {
            // Arrange
            _mockStoreClient.Setup(m => m.GetProductByIdAsync(It.IsAny<int>()))
                .Throws(new Exception());

            // Act
            var product = await _bazzasBazaarService.GetProduct(It.IsAny<int>());

            // Assert
            Assert.IsNull(product);
        }

        [Test]
        public async Task GetAllProducts_Success()
        {
            // Arrange
            _mockStoreClient.Setup(m => m.GetAllCategoriesAsync())
                .ReturnsAsync(_stubCategories);
            _mockStoreClient.Setup(m => m.GetFilteredProductsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(_stubProducts);

            // Act
            var products = await _bazzasBazaarService.GetAllProducts();

            // Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(products.First().Id, _stubProducts[0].Id);
        }
    }
}