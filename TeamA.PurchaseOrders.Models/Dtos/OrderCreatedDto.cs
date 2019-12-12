using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class OrderCreatedDto
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string CardNumber { get; set; }
        public int ProductId { get; set; }
        public int Quantity {get; set;}
        public DateTime PurchasedOn { get; set; }
        public string ProductName { get; set; }
        public string ProductEan { get; set; }
        public double TotalPrice { get; set; }
    }
}
