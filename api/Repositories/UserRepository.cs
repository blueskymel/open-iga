using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;

namespace OpenIga.Api.Repositories;

public class UserRepository(OpenIgaDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyCollection<User>> GetAllAsync()
    {
        return await dbContext.Users.AsNoTracking().ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id, bool trackChanges = false)
    {
        var query = trackChanges ? dbContext.Users : dbContext.Users.AsNoTracking();
        return await query.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await dbContext.Users.AnyAsync(user => user.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await dbContext.Users.AnyAsync(user => user.Email == email);
    }

    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }

    public void Remove(User user)
    {
        dbContext.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
