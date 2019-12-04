using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class UndercuttersService : IUndercuttersService
    {
        private HttpClient _client;
        public UndercuttersService(HttpClient client)
        {
            _client = client;
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
    }
}
