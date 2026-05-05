using OpenIga.Api.Models;

namespace OpenIga.Api.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id, bool trackChanges = false);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    void Add(User user);
    void Remove(User user);
    Task SaveChangesAsync();
}
