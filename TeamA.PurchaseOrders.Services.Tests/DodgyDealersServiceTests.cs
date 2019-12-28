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

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
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
            
        }

        
    }
}