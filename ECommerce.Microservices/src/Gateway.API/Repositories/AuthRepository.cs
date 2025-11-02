using Microsoft.EntityFrameworkCore;
using Gateway.API.Data;
using Gateway.API.Models;

namespace Gateway.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly GatewayDbContext _context;

        public AuthRepository(GatewayDbContext context)
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
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}