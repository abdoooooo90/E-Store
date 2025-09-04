using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Categories
{
    public class CategoryRepository: GenericRepository<Category>, ICatigoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext): base(dbContext)
        {
            

        }

    }
}
