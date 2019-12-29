using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Services;

namespace TeamA.PurchaseOrders.Services.Tests
{
    public class DodgyDealersServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<ILogger<DodgyDealersService>> _mockLogger;
        private DodgyDealersService _dodgyDealersService;
        private ExternalProductDto _stubExternalProductDto;
        private List<ExternalProductDto> _stubExternalProductDtoList;
        private OrderCreatedDto _stubOrderCreatedDto;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClient.BaseAddress = new Uri("http://somerandomapi.com");
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<DodgyDealersService>>();
            _dodgyDealersService = new DodgyDealersService(_httpClient, _mockHttpClientFactory.Object, _mockLogger.Object);
            _stubExternalProductDto = new ExternalProductDto
            {
                BrandId = 1,
                BrandName = "Fake Brands",
                CategoryId = 1,
                CategoryName = "Fake Categories",
                Description = "Fakest description",
                Ean = "11 1  1 1 ",
                ExpectedRestock = false,
                Id = 1,
                InStock = true,
                Name = "Fake!!",
                Price = 10.00,
                Source = "DodgyDealers"
            };
            _stubExternalProductDtoList = new List<ExternalProductDto>() { _stubExternalProductDto };
            _stubOrderCreatedDto = new OrderCreatedDto
            {
                AccountName = "Oli",
                CardNumber = "20492094024902492",
                Id = 1,
                ProductEan = "1-309-24",
                ProductId = 1,
                ProductName = "Olis product",
                PurchasedOn = new DateTime(),
                Quantity = 5,
                Success = true,
                TotalPrice = 10.00
            };
        }

        [Test]
        public async Task GetProducts_Success()
        {
            // Arrange
            _mockHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(_stubExternalProductDtoList), Encoding.UTF8, "application/json")
                }).Verifiable();


            // Act
            var result = await _dodgyDealersService.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.First().Id, _stubExternalProductDtoList[0].Id);
            Assert.AreEqual(result.First().BrandName, _stubExternalProductDtoList[0].BrandName);
            Assert.AreEqual(result.First().CategoryId, _stubExternalProductDtoList[0].CategoryId);
            Assert.AreEqual(result.First().CategoryName, _stubExternalProductDtoList[0].CategoryName);
            Assert.AreEqual(result.First().Description, _stubExternalProductDtoList[0].Description);
            Assert.AreEqual(result.First().Ean, _stubExternalProductDtoList[0].Ean);
            Assert.AreEqual(result.First().ExpectedRestock, _stubExternalProductDtoList[0].ExpectedRestock);
            Assert.AreEqual(result.First().InStock, _stubExternalProductDtoList[0].InStock);
            Assert.AreEqual(result.First().Price, _stubExternalProductDtoList[0].Price);
            Assert.AreEqual(result.First().Source, _stubExternalProductDtoList[0].Source);
        }

        [Test]
        public async Task GetProducts_Fails_EmptyList()
        {
            // Arrange
            _mockHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                }).Verifiable();

            // Act
            var product = await _dodgyDealersService.GetProducts();

            // Assert
            Assert.IsNotNull(product);
            Assert.IsInstanceOf<List<ExternalProductDto>>(product);
            Assert.AreEqual(product, new List<ExternalProductDto>());
        }

        [Test]
        public async Task GetProductById_Success()
        {
            // Arrange
            _mockHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(_stubExternalProductDtoList.First()), Encoding.UTF8, "application/json")
                }).Verifiable();

            // Act
            var product = await _dodgyDealersService.GetProduct(It.IsAny<int>());

            //Assert
            Assert.IsNotNull(product);
            Assert.IsInstanceOf<ExternalProductDto>(product);
            Assert.AreEqual(product.Id, _stubExternalProductDtoList.First().Id);
            Assert.AreEqual(product.BrandName, _stubExternalProductDtoList.First().BrandName);
            Assert.AreEqual(product.CategoryId, _stubExternalProductDtoList.First().CategoryId);
            Assert.AreEqual(product.CategoryName, _stubExternalProductDtoList.First().CategoryName);
            Assert.AreEqual(product.Description, _stubExternalProductDtoList.First().Description);
            Assert.AreEqual(product.Ean, _stubExternalProductDtoList.First().Ean);
            Assert.AreEqual(product.ExpectedRestock, _stubExternalProductDtoList.First().ExpectedRestock);
            Assert.AreEqual(product.InStock, _stubExternalProductDtoList.First().InStock);
            Assert.AreEqual(product.Price, _stubExternalProductDtoList.First().Price);
            Assert.AreEqual(product.Source, _stubExternalProductDtoList.First().Source);
        }

        [Test]
        public async Task GetProductById_Fails_Null()
        {
            // Arrange
            _mockHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                }).Verifiable();

            // Act
            var product = await _dodgyDealersService.GetProduct(It.IsAny<int>());

            // Assert
            Assert.IsNull(product);
        }

        [Test]
        public async Task CreateOrder_Valid_Success()
        {
            // Arrange
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(_stubOrderCreatedDto), Encoding.UTF8, "application/json")
                }).Verifiable();

            // Act
            var order = await _dodgyDealersService.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(order);
            Assert.IsInstanceOf<OrderCreatedDto>(order);
            Assert.AreEqual(order.Id, _stubOrderCreatedDto.Id);
            Assert.AreEqual(order.AccountName, _stubOrderCreatedDto.AccountName);
            Assert.AreEqual(order.CardNumber, _stubOrderCreatedDto.CardNumber);
            Assert.AreEqual(order.ProductEan, _stubOrderCreatedDto.ProductEan);
            Assert.AreEqual(order.ProductId, _stubOrderCreatedDto.ProductId);
            Assert.AreEqual(order.ProductName, _stubOrderCreatedDto.ProductName);
            Assert.AreEqual(order.PurchasedOn, _stubOrderCreatedDto.PurchasedOn);
            Assert.AreEqual(order.Quantity, _stubOrderCreatedDto.Quantity);
            Assert.AreEqual(order.Success, _stubOrderCreatedDto.Success);
            Assert.AreEqual(order.TotalPrice, _stubOrderCreatedDto.TotalPrice);
        }

        [Test]
        public async Task CreateOrder_Forbidden_InsufficientStock()
        {
            // Arrange
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Content = new StringContent(JsonConvert.SerializeObject("insufficient stock"), Encoding.UTF8, "application/json")
                }).Verifiable();

            // Act
            var order = await _dodgyDealersService.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(order);
            Assert.IsInstanceOf<OrderCreatedDto>(order);
            Assert.IsFalse(order.Success);
        }

        [Test]
        public async Task CreateOrder_Fails_Fails()
        {
            // Arrange
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(JsonConvert.SerializeObject("error"), Encoding.UTF8, "application/json")
                }).Verifiable();

            // Act
            var order = await _dodgyDealersService.CreateOrder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(order);
            Assert.IsInstanceOf<OrderCreatedDto>(order);
            Assert.IsFalse(order.Success);
        }   
    }
}