﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
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

        private IHttpClientFactory _clientFactory;
        public UndercuttersService(HttpClient client, IHttpClientFactory clientFactory)
        {
            _client = client;
            _clientFactory = clientFactory;
        }

        public async Task<List<ExternalProductDto>> GetProducts()
        {
            try
            {
                using (var client = _clientFactory.CreateClient("background"))
                {
                    using (HttpResponseMessage response = await _client.GetAsync("http://undercutters.azurewebsites.net/api/product"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var products = await response.Content.ReadAsAsync<List<ExternalProductDto>>();
                            foreach (var product in products)
                            {
                                product.Source = "Undercutters";
                            }
                            return products;

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public async Task<ExternalProductDto> GetProduct(int id)
        {
            try
            {
                using (HttpResponseMessage response = await _client.GetAsync($"http://undercutters.azurewebsites.net/api/product?id={id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var product = await response.Content.ReadAsAsync<ExternalProductDto>();
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
                    if(response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        if (responseString.Contains("insufficient stock"))
                        {
                            return new OrderCreatedDto
                            {
                                Success = false
                            };
                        }
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
