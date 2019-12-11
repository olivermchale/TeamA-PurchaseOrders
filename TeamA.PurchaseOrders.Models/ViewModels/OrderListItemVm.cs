using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.ViewModels
{
    public class OrderListItemVm
    {
        public Guid Id { get; set; }

        public string OrderStatus { get; set; }

        public string ProductName { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
    }
}
