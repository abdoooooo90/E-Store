using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.UserServices
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string fullName, string email, string password);
        Task<User?> LoginAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
