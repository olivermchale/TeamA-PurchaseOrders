using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Models.Dtos;
using TeamA.PurchaseOrders.Repository.Interfaces;

namespace TeamA.PurchaseOrders.Repository.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private PurchaseOrdersDb _context;
        public ProductsRepository(PurchaseOrdersDb context)
        {
            _context = context;
        }
        public async Task<List<ProductDto>> GetProducts()
        {
            return await _context.Products.Select(p => new ProductDto
            {
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName,
                Description = p.Description,
                Ean = p.Ean,
                ExpectedRestock = p.ExpectedRestock,
                Id = p.Id,
                InStock = p.InStock,
                Name = p.Name,
                Price = p.Price,
                Source = p.Source,
                ExternalId = p.ExternalId
            }).ToListAsync();
        }

        public async Task<bool> SaveProducts(IEnumerable<ExternalProductDto> products)
        {
            foreach(var product in products)
            {
                if(!_context.Products.Any(p => p.Ean == product.Ean && p.Source == product.Source))
                {
                    // Product doesnt exist, add it
                    var item = new ProductDto
                    {
                        Id = Guid.NewGuid(),
                        InStock = product.InStock,
                        CategoryId = product.CategoryId,
                        CategoryName = product.CategoryName,
                        Description = product.Description,
                        Ean = product.Ean,
                        ExpectedRestock = product.ExpectedRestock,
                        ExternalId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Source = product.Source
                    };
                    await _context.Products.AddAsync(item);
                }
                else
                {
                    // Product exists, lets update it
                    var existingProduct = await _context.Products.Where(s => s.Ean == product.Ean && s.Source == product.Source).FirstOrDefaultAsync();
                    existingProduct.InStock = product.InStock;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.CategoryName = product.CategoryName;
                    existingProduct.Description = product.Description;
                    existingProduct.Ean = product.Ean;
                    existingProduct.ExpectedRestock = product.ExpectedRestock;
                    existingProduct.ExternalId = product.Id;
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Source = product.Source;

                     _context.Update(existingProduct);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductDto>> GetProductsByEan(string ean)
        {
            if (ean == null)
            {
                return null;
            }
            try
            {
                var products = await _context.Products.Where(p => p.Ean == ean).ToListAsync();
                return products;
            }
            catch(Exception e)
            {
                // todo: exception handling
            }
            return null;
        }
    }
}
