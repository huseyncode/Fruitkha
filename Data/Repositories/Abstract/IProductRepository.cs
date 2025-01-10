﻿using Core.Entities;
using Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Abstract;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product> GetProductWithCategoriesAsync(int id);
    Task<List<Product>> GetAllProductsWithCategoriesAsync();
    Task<Product> GetProductByTitleAsync(string title);
    Task<List<Product>> GetLastThreeProducts();
    IQueryable<Product> FilterByName(string name);
    IQueryable<Product> FilterByCount(IQueryable<Product> products, int? minCount, int? maxCount);
    IQueryable<Product> FilterByPrice(IQueryable<Product> products, int? minPrice, int? maxPrice);
}
