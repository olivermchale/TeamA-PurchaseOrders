using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class PurchaseStatusDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = "Ordered";
    }
}
