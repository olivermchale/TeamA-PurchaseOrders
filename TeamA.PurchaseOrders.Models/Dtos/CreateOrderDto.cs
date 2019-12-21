using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class CreateOrderDto
    {
        public string AccountName { get; set; }
        public string CardNumber { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
