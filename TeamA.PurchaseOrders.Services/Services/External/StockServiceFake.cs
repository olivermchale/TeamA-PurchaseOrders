using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Services.Interfaces.External;

namespace TeamA.PurchaseOrders.Services.Services.External
{
    public class StockServiceFake : IStockService
    {
        public async Task<bool> UpdateStockLevel(SetStockLevelDto setStockLevelDto, string token)
        {
            return await Task.FromResult(true);
        }
    }
}
