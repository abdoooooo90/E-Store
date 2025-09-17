using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Products
{
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbcontext): base(dbcontext)
        {
            
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
