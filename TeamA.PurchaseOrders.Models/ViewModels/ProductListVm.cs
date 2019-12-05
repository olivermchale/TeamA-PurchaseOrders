using System;
using System.Collections.Generic;
using System.Text;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Models.ViewModels
{
    public class ProductListVm
    {
        public string Source { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
