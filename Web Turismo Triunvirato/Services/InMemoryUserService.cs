using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models;

namespace Web_Turismo_Triunvirato.Services
{
    public class InMemoryUserService : IUserService
    {
        private readonly List<User> _users = new List<User>();
        private int _nextId = 1;

        public Task AddAsync(User user)
        {
            user.Id = _nextId++;
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var userToRemove = _users.FirstOrDefault(u => u.Id == id);
            if (userToRemove != null)
            {
                _users.Remove(userToRemove);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult(_users.AsEnumerable());
        }

        public Task<User> GetByIdAsync(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task UpdateAsync(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Name = user.Name;
                existingUser.Surname = user.Surname;
                //existingUser.Pais = user.Pais;
            }
            return Task.CompletedTask;
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
        }
    }
}