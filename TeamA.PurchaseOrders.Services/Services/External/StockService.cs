using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Interfaces.External;

namespace TeamA.PurchaseOrders.Services.Services.External
{
    public class StockService : IStockService
    {
        private readonly HttpClient _client;
        private readonly ILogger<StockService> _logger;
        public StockService(HttpClient httpClient, ILogger<StockService> logger)
        {
            _client = httpClient;
            _logger = logger;
        }
        public async Task<bool> UpdateStockLevel(SetStockLevelDto setStockLevelDto, string token)
        {
            try
            {
                _client.DefaultRequestHeaders.Add("Authorization", token);
                _logger.LogInformation("Updating stock level for product: " + setStockLevelDto.ProductID);
                var json = new StringContent(JsonConvert.SerializeObject(setStockLevelDto), Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await _client.PostAsync("api/Stock/SetStockLevelOfStock", json))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Successfully updated stock for product: " + setStockLevelDto.ProductID);
                        return true;
                    }
                    _logger.LogError("Error while updating stock: " + response);
                    return false;
                }
            }
            catch(Exception e)
            {
                _logger.LogError("Error while updating stock for product: " + setStockLevelDto.ProductID + e + e.StackTrace);
                return false;
            }
        }
    }
}
