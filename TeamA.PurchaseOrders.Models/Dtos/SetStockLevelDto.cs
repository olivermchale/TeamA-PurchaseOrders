using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class SetStockLevelDto
    {
        public Guid ProductID { get; set; }

        public int StockLevel { get; set; }
    }
}
