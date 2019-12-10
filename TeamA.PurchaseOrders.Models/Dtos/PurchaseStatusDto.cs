using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class PurchaseStatusDto
    {
        public Guid ID { get; set; } = new Guid();

        public string Name { get; set; } = "Ordered";
    }
}
