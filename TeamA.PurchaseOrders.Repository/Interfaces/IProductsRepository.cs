using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Repository.Interfaces
{
    public interface IProductsRepository
    {
        Task<List<ProductDto>> GetProducts();

        Task<bool> SaveProducts(IEnumerable<ExternalProductDto> products);

        Task<List<ProductDto>> GetProductsByEan(string ean);
    }
}
