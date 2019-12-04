﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Services
{
    public class DodgyDealersService : IDodgyDealersService
    {
        private HttpClient _client;
        public DodgyDealersService(HttpClient client)
        {
            _client = client;
        }
        public async Task<ProductArrayDto> GetProducts()
        {
            try
            {
                using (HttpResponseMessage response = await _client.GetAsync("api/product"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var xmlString = await response.Content.ReadAsStringAsync();
                        XmlSerializer serializer = new XmlSerializer(typeof(ProductDto));
                        using (StringReader reader = new StringReader(xmlString))
                        {
                           return (ProductArrayDto)(serializer.Deserialize(reader));
                        }
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
