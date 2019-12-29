using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class DodgyDealersService : IDodgyDealersService, IOrdersService
    {
        private HttpClient _client;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<DodgyDealersService> _logger;

        public DodgyDealersService()
        {

        }
        public DodgyDealersService(HttpClient client, IHttpClientFactory clientFactory, ILogger<DodgyDealersService> logger)
        {
            _client = client;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        public async Task<List<ExternalProductDto>> GetProducts()
        {
            _logger.LogInformation("Getting all products - Dodgy Dealers service");
            try
            {
                using (var client = _clientFactory.CreateClient("background"))
                {
                    using (HttpResponseMessage response = await client.GetAsync("http://dodgydealers.azurewebsites.net/api/product"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Successfully got all products");
                            var products = await response.Content.ReadAsAsync<List<ExternalProductDto>>();
                            foreach (var product in products)
                            {
                                product.Source = "DodgyDealers";
                            }
                            return products;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception when getting products" + e + e.StackTrace);
            }
            _logger.LogDebug("Failed to get products");
            return new List<ExternalProductDto>();
        }

        public async Task<ExternalProductDto> GetProduct(int id)
        {
            _logger.LogInformation("Getting product with id: " + id);
            try
            {
                using (var client = _clientFactory.CreateClient("background"))
                {
                    // todo: env variable?
                    using (HttpResponseMessage response = await client.GetAsync($"http://dodgydealers.azurewebsites.net/api/product?id={id}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Successfully recieved products");
                            var product = await response.Content.ReadAsAsync<ExternalProductDto>();
                            return product;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception when getting product with id: " + id + e + e.StackTrace);
            }
            return null;
        }

        public async Task<OrderCreatedDto> CreateOrder(string accountName, string cardNumber, int productId, int quantity)
        {
            _logger.LogInformation("Creating order for product with id: " + productId);
            var order = new CreateOrderDto()
            {
                AccountName = accountName,
                CardNumber = cardNumber,
                ProductId = productId,
                Quantity = quantity
            };
            var json = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
            try
            {
                using (HttpResponseMessage response = await _client.PostAsync("api/order/", json))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Successfully created order for product with Id: " + productId);
                        var product = await response.Content.ReadAsAsync<OrderCreatedDto>();
                        return product;
                    }
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        if (responseString.Contains("insufficient stock"))
                        {
                            _logger.LogInformation("Failed to create order due to insufficient stock with product ID" + productId);
                            return new OrderCreatedDto
                            {
                                Success = false
                            };
                        }
                    }
                    _logger.LogDebug("Failed to create order for product with id " + productId);
                    return new OrderCreatedDto
                    {
                        Success = false
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception when getting product with id: " + productId + e + e.StackTrace);
            }
            return null;
        }
    }
}
