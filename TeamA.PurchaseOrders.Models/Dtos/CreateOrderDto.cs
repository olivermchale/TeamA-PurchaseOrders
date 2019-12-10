using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public DateTime CardExpiry { get; set; }
        public string CardCVC { get; set; }
        public int Quantity { get; set; }
        public double ProductPrice { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
