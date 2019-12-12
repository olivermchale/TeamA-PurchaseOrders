using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Models.ViewModels;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class UndercuttersService : IUndercuttersService, IOrdersService
    {
        private HttpClient _client;
        public UndercuttersService(HttpClient client)
        {
            _client = client;
        }

        public UndercuttersService()
        {
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            try
            {
                using (HttpResponseMessage response = await _client.GetAsync("api/product"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var products = await response.Content.ReadAsAsync<List<ProductDto>>();
                        return products;
                        
                    }
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            try
            {
                using (HttpResponseMessage response = await _client.GetAsync($"api/product?id={id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var product = await response.Content.ReadAsAsync<ProductDto>();
                        return product;

                    }
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public async Task<OrderCreatedDto> CreateOrder(string accountName, string cardNumber, int productId, int quantity)
        {
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
                    if(response.IsSuccessStatusCode)
                    {
                        var product = await response.Content.ReadAsAsync<OrderCreatedDto>();
                        return product;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                //todo: exception handling
            }
            return null;
        }
    }
}
