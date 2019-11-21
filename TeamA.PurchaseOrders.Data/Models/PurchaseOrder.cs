using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Data.Models
{
    public class PurchaseOrder
    {
        public Guid ID { get; set; }

        public Guid PurchasedBy { get; set; }

        public Guid ProductID { get; set; }

        public DateTime? PurchasedOn { get; set; }

        public Guid StatusID { get; set; }

        public Guid PaymentInformationID { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public double Cost { get; set; }

        public bool IsDeleted { get; set; }

        public PaymentInformation PaymentInformation { get; set; }

        public PurchaseStatus PurchaseStatus { get; set; }

    }
}
