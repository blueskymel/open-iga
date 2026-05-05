using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;

namespace OpenIga.Api.Repositories;

public class AccessRequestRepository(OpenIgaDbContext dbContext) : IAccessRequestRepository
{
    public async Task<IReadOnlyCollection<AccessRequest>> GetAllAsync()
    {
        return await dbContext.AccessRequests.AsNoTracking().ToListAsync();
    }

    public async Task<AccessRequest?> GetByIdAsync(Guid id, bool trackChanges = false)
    {
        var query = trackChanges ? dbContext.AccessRequests : dbContext.AccessRequests.AsNoTracking();
        return await query.FirstOrDefaultAsync(accessRequest => accessRequest.Id == id);
    }

    public async Task<bool> PendingRequestExistsAsync(Guid userId, Guid roleId)
    {
        return await dbContext.AccessRequests.AnyAsync(accessRequest =>
            accessRequest.UserId == userId
            && accessRequest.RoleId == roleId
            && accessRequest.Status == AccessRequestStatus.Pending);
    }

    public void Add(AccessRequest accessRequest)
    {
        dbContext.AccessRequests.Add(accessRequest);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
