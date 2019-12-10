using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class PurchaseOrderDto
    {
        public Guid ID { get; set; }

        public Guid PurchasedBy { get; set; }

        public int ProductID { get; set; }

        public DateTime? PurchasedOn { get; set; }

        public Guid StatusID { get; set; }

        public Guid PaymentInformationID { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public string Address { get; set; }

        public string Postcode { get; set; }

        public double ProductPrice { get; set; }

        public bool IsDeleted { get; set; } = false;

        public PaymentInformationDto PaymentInformation { get; set; }

        public PurchaseStatusDto PurchaseStatus { get; set; }

    }
}
