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

        public Guid ProductID { get; set; }

        public DateTime? PurchasedOn { get; set; }

        public Guid StatusID { get; set; }

        public Guid PaymentInformationID { get; set; }

        public int ExternalID { get; set; }

        [Required(ErrorMessage ="Product Name is Required"), MinLength(2, ErrorMessage ="Invalid product name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage ="Quantity is required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Product Price is required"), DataType(DataType.Currency, ErrorMessage ="Invalid Currency")]
        public double ProductPrice { get; set; }

        [Required(ErrorMessage = "Address is required"), DataType(DataType.Text), MinLength(1, ErrorMessage="Address is required with a minumum length of 1")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Postcode is required"), DataType(DataType.PostalCode), MinLength(1, ErrorMessage = "Postcode is required with a minumum length of 1")]
        public string Postcode { get; set; }

        public string Source { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Required(ErrorMessage = "Insufficient payment information")]
        public PaymentInformationDto PaymentInformation { get; set; }

        public PurchaseStatusDto PurchaseStatus { get; set; }

    }
}
