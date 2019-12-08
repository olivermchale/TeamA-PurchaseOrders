using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class ProductDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Ean { get; set; }
        public bool? ExpectedRestock { get; set; }
        public int Id { get; set; }
        public bool InStock { get; set; } = false;
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
