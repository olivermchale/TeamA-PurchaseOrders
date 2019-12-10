using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Models.Dtos
{
    public class PaymentInformationDto
    {
        public Guid ID { get; set; } = new Guid();

        public string CardNumber { get; set; }

        public DateTime CardExpiry { get; set; }

        public string CardCVC { get; set; }

        public string CardName { get; set; }

    }
}
