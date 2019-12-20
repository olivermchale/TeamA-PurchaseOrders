using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.ViewModels
{
    public class OrderDetailVm
    {
        public Guid Id { get; set; } 
        public Guid ProductId { get; set; }
        public DateTime? PurchasedOn { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double OrderPrice { get; set; }
        public double ProductPrice { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Source { get; set; }
        public string Last4Digits { get; set; }
        public string CardholderName { get; set; }
        public string PurchaseStatus { get; set; }
    }
}
