using Estoque.API.Models;
using Microsoft.EntityFrameworkCore;
using Estoque.API.Data;

namespace Estoque.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EstoqueDbContext _context;
        public AuthRepository(EstoqueDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}