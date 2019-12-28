using System;
using System.Collections.Generic;
using System.Text;
using TeamA.PurchaseOrders.Services.Interfaces;

namespace TeamA.PurchaseOrders.Services.Factories
{
    public interface IOrdersFactory
    {
        IOrdersService Create(string serviceType);
    }
}
