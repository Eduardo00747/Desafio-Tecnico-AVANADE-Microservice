using Estoque.API.Models;
using System.Threading.Tasks;

namespace Estoque.API.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserAsync(string username, string password);

        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
    }
}