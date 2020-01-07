using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Services.Interfaces.External
{
    public interface IStockService
    {
        Task<bool> UpdateStockLevel(SetStockLevelDto setStockLevelDto, string token);
    }
}
