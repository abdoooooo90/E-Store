using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Generic
{
    public interface IGenericRepository <T> where T : BaseEntity
    {   
        Task<IEnumerable<T>> GetAllAsync(bool withAsNoTracking = true);
        Task<T?> GetByIdAsync(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<int> SaveChangesAsync();
    }
}
