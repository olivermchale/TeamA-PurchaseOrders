using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class ProductArrayDto
    {
        public ProductDto[] products { get; set; }
    }
}
