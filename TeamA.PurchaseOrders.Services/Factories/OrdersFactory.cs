using System;
using System.Collections.Generic;
using System.Text;
using TeamA.PurchaseOrders.Services.Interfaces;
using TeamA.PurchaseOrders.Services.Services;

namespace TeamA.PurchaseOrders.Services.Factories
{
    public class OrdersFactory : IOrdersFactory
    {
        private IUndercuttersService _undercuttersService;
        private IDodgyDealersService _dodgyDealersService;
        private IBazzasBazaarService _bazzasBazaarService;

        public OrdersFactory(IUndercuttersService undercuttersService, IDodgyDealersService dodgyDealersService, IBazzasBazaarService bazzasBazaarService)
        {
            _undercuttersService = undercuttersService;
            _dodgyDealersService = dodgyDealersService;
            _bazzasBazaarService = bazzasBazaarService;
        }
        public IOrdersService Create(string serviceType)
        {
            switch (serviceType)
            {
                case "Undercutters":
                    return _undercuttersService;
                case "DodgyDealers":
                     return _dodgyDealersService;
                case "BazzasBazaar":
                    return _bazzasBazaarService;
                default:
                    throw new NotImplementedException();
            }

        }
    }
}
