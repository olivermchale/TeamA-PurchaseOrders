using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.PurchaseOrders.Data.Models
{
    public class PaymentInformation
    {
        public Guid ID { get; set; }

        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public string CVC { get; set; }

        public string CardholderName { get; set; }

    }
}
