using Common.Entities;
using Data.Context;
using Data.Repositeries.Abstract;
using Data.Repositeries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositeries.Concrete;
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {

    }

}
