using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class PaymentInformationDto
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required, MinLength(16), MaxLength(16), DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }

        [Required]
        public DateTime CardExpiry { get; set; }

        [Required, MinLength(3)]
        public string CardCVC { get; set; }

        [Required, MinLength(2)]
        public string CardName { get; set; }

    }
}
