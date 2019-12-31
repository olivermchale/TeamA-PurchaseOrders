using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Repository.Repositories;

namespace TeamA.PurchaseOrders.Repository.Tests
{
    public class ProductsRepositoryTests
    {
        private IProductsRepository _productsRepository;
        private Mock<ILogger<ProductsRepository>> _mockLogger;
        private PurchaseStatusDto _stubPurchaseStatusDto;
        private PaymentInformationDto _stubPaymentInformationDto;
        private PurchaseOrderDto _stubPurchaseOrderDto;
        private OrderCreatedDto _stubOrderCreatedDto;
        private ProductDto _stubProductDto;
        
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
            _stubProductDto = new ProductDto
            {
                CategoryId = 1,
                CategoryName = "Cat",
                Description = "Desc",
                Ean = "1 1 1 2 123",
                ExpectedRestock = true,
                ExternalId = 1,
                Id = Guid.Parse("d61a78a9-b6ad-4430-91ea-0c8d5227b622"),
                InStock = true,
                Name = "Prod",
                Price = 10,
                Source = "Undercutters"
            };
            _mockLogger = new Mock<ILogger<ProductsRepository>>();
            _productsRepository = new ProductsRepository(GetInMemoryContextWithSeedData(), _mockLogger.Object);
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
            context.Products.Add(_stubProductDto);

            context.SaveChanges();

            return context;
        }

        [Test]
        public async Task GetProducts_Success()
        {
            // Act
            var products = await _productsRepository.GetProducts();

            // Assert
            Assert.IsNotNull(products);
            Assert.IsInstanceOf<ProductDto>(products.First());
            Assert.AreEqual(products.First().Id, _stubProductDto.Id);
            Assert.AreEqual(products.First().CategoryId, _stubProductDto.CategoryId);
            Assert.AreEqual(products.First().Description, _stubProductDto.Description);
            Assert.AreEqual(products.First().Ean, _stubProductDto.Ean);
            Assert.AreEqual(products.First().ExpectedRestock, _stubProductDto.ExpectedRestock);
            Assert.AreEqual(products.First().ExternalId, _stubProductDto.ExternalId);
            Assert.AreEqual(products.First().Id, _stubProductDto.Id);
            Assert.AreEqual(products.First().InStock, _stubProductDto.InStock);
            Assert.AreEqual(products.First().Name, _stubProductDto.Name);
            Assert.AreEqual(products.First().Price, _stubProductDto.Price);
            Assert.AreEqual(products.First().Source, _stubProductDto.Source);
        }

        [Test]
        public async Task SaveProducts_Success()
        {
            var stubProduct = new ExternalProductDto
            {
                BrandId = 1,
                BrandName = "Brand",
                CategoryId = 1,
                CategoryName = "Cat",
                Description = "Desc",
                Ean = "1 1 1 2 123",
                ExpectedRestock = true,
                Id = 1,
                InStock = true,
                Name = "Prod",
                Price = 10,
                Source = "Undercutters"
            };
            var stubList = new List<ExternalProductDto>() { stubProduct };
            // Act
            var success = await _productsRepository.SaveProducts(stubList);
            // Assert
            Assert.IsNotNull(success);
            Assert.IsTrue(success);
        }

        [Test]
        public async Task GetProductsByEan_Success()
        {
            var products = await _productsRepository.GetProductsByEan("1 1 1 2 123");

            Assert.IsNotNull(products);
            Assert.IsInstanceOf<List<ProductDto>>(products);
            Assert.AreEqual(products.First().Id, _stubProductDto.Id);
            Assert.AreEqual(products.First().BrandId, _stubProductDto.BrandId);
            Assert.AreEqual(products.First().BrandName, _stubProductDto.BrandName);
            Assert.AreEqual(products.First().CategoryId, _stubProductDto.CategoryId);
            Assert.AreEqual(products.First().Description, _stubProductDto.Description);
            Assert.AreEqual(products.First().Ean, _stubProductDto.Ean);
            Assert.AreEqual(products.First().ExpectedRestock, _stubProductDto.ExpectedRestock);
            Assert.AreEqual(products.First().ExternalId, _stubProductDto.ExternalId);
            Assert.AreEqual(products.First().Id, _stubProductDto.Id);
            Assert.AreEqual(products.First().InStock, _stubProductDto.InStock);
            Assert.AreEqual(products.First().Name, _stubProductDto.Name);
            Assert.AreEqual(products.First().Price, _stubProductDto.Price);
            Assert.AreEqual(products.First().Source, _stubProductDto.Source);
        }

        [Test]
        public async Task GetProductsByEan_NoEan_Fails_Null()
        {
            var products = await _productsRepository.GetProductsByEan(string.Empty);

            Assert.IsNull(products);
        }

        [Test]
        public async Task GetProductsByEan_NoProductsFound_EmptyT()
        {
            var products = await _productsRepository.GetProductsByEan("some fake ean");
            Assert.IsNotNull(products);
            Assert.IsInstanceOf<List<ProductDto>>(products);
            Assert.AreEqual(0, products.Count());
        }
    }
}