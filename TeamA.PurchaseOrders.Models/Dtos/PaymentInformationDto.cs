using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class PaymentInformationDto
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Card number is required"), 
            MinLength(16, ErrorMessage ="Invalid card number"), 
            MaxLength(16, ErrorMessage = "Invalid card number"), 
            DataType(DataType.CreditCard, ErrorMessage = "Invalid card number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Card Expiry date is required (format: MM/YY)"), DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        public DateTime CardExpiry { get; set; }

        [Required(ErrorMessage = "Card CVC date is required"), MinLength(3, ErrorMessage="CVC requires a minimum length of 3")]
        public string CardCVC { get; set; }

        [Required(ErrorMessage = "Cardholder Name is required"), MinLength(2, ErrorMessage ="Invalid name, minimum length 2")]
        public string CardName { get; set; }

    }
}
