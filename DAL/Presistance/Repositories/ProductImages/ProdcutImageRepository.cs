using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.ProductImages
{
    public class ProdcutImageRepository: GenericRepository<ProductImage>, IProductImagesRepository
    {
        public ProdcutImageRepository(ApplicationDbContext dbcontext): base(dbcontext)
        {
            
        }
    }
}
