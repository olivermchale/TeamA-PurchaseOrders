using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Interfaces;
using TeamA.PurchaseOrdersAPI.Controllers;

namespace TeamA.PurchaseOrders.API.Tests
{
    public class ProductsControllerTests
    {
        private ProductsController _productsController;
        private Mock<IProductsService> _mockProductsService;
        private Mock<ILogger<ProductsController>> _mockLogger;
        private ProductDto _mockProductDto;
        private List<ProductDto> _mockProductDtoList;
        [SetUp]
        public void Setup()
        {
            _mockProductsService = new Mock<IProductsService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _productsController = new ProductsController(_mockProductsService.Object, _mockLogger.Object);
            _mockProductDto = new ProductDto
            {
                BrandId = 10,
                BrandName = "Olis Stuff",
                CategoryId = 10,
                CategoryName = "Olis stuffs",
                Description = "Stuff from Oli",
                Ean = "1 01010 1",
                ExpectedRestock = true,
                ExternalId = 5,
                Id = Guid.NewGuid(),
                InStock = true,
                Name = "Olis Item",
                Price = 10.00,
                Source = "Oli"
            };
            _mockProductDtoList = new List<ProductDto>();
            _mockProductDtoList.Add(_mockProductDto);
        }

        [Test]
        public async Task GetAndSaveProducts_Success()
        {
            // Arrange
            _mockProductsService.Setup(m => m.GetAndSaveProducts())
                .ReturnsAsync(true);

            // Act
            var result = await _productsController.GetAndSaveProducts() as OkObjectResult;
            Assert.AreEqual(result.StatusCode, 200);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value, true);
            _mockProductsService.Verify(m => m.GetAndSaveProducts(), Times.Once);
        }

        [Test]
        public async Task GetAndSaveProducts_FailToGetProducts_NotFound()
        {
            // Arrange 
            _mockProductsService.Setup(m => m.GetAndSaveProducts())
                .ReturnsAsync(false);

            // Act
            var result = await _productsController.GetAndSaveProducts() as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 404);
            _mockProductsService.Verify(m => m.GetAndSaveProducts(), Times.Once);
        }

        [Test]
        public async Task GetSavedProducts_Success()
        {
            // Arrange
            _mockProductsService.Setup(m => m.GetProducts())
                .ReturnsAsync(_mockProductDtoList);

            // Act
            var result = await _productsController.GetSavedProducts() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, _mockProductDtoList);
            _mockProductsService.Verify(m => m.GetProducts(), Times.Once);
        }

        [Test]
        public async Task GetSavedProducts_NoProducts_NotFound()
        {
            // Arrange
            _mockProductsService.Setup(m => m.GetProducts())
                .ReturnsAsync(() => null);

            // Act
            var result = await _productsController.GetSavedProducts() as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 404);
            _mockProductsService.Verify(m => m.GetProducts(), Times.Once);
        }

        [Test]
        public async Task GetProductByEan_Success()
        {
            // Arrange
            _mockProductsService.Setup(m => m.GetProductsByEan(It.IsAny<string>()))
                .ReturnsAsync(_mockProductDtoList);

            // Act
            var result = await _productsController.GetProducts("ean") as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, _mockProductDtoList);
            _mockProductsService.Verify(m => m.GetProductsByEan(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetProductByEan_NoEan_BadRequest()
        {
            // Arrange
            _mockProductsService.Setup(m => m.GetProductsByEan(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _productsController.GetProducts(null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.Value, "No ean");
            _mockProductsService.Verify(m => m.GetProductsByEan(null), Times.Never);
        }

        [Test]
        public async Task GetProductsByEan_NoProducts_NotFound()
        {
            // Arrange
            _mockProductsService.Setup(m => m.GetProductsByEan(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _productsController.GetProducts("ean") as NotFoundResult;

            // Assert 
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 404);
            _mockProductsService.Verify(m => m.GetProductsByEan(It.IsAny<string>()), Times.Once);
        }
    }
}