using System;
using System.Collections.Generic;
using System.Text;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Models.ViewModels
{
    public class ProductItemVm
    {
        public string Source { get; set; }
        public ExternalProductDto Product {get; set;}
    }
}
