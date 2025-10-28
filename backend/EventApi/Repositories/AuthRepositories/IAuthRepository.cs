using EventApi.Models;

namespace EventApi.Repositories.AuthRepositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(User user);
    }
}