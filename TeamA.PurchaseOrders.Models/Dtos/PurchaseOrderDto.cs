using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public int ExternalID { get; set; }

        [Required, MinLength(2)]
        public string ProductName { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required, DataType(DataType.Currency)]
        public double ProductPrice { get; set; }

        [Required, DataType(DataType.Text)]
        public string Address { get; set; }
        [Required, DataType(DataType.PostalCode)]
        public string Postcode { get; set; }

        public string Source { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Required]
        public PaymentInformationDto PaymentInformation { get; set; }

        public PurchaseStatusDto PurchaseStatus { get; set; }

    }
}
