using System.Collections.Generic;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models;

namespace Web_Turismo_Triunvirato.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User> GetByEmailAsync(string email); 
    }
}