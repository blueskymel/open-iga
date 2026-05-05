using OpenIga.Api.Models;

namespace OpenIga.Api.Repositories;

public interface IAccessRequestRepository
{
    Task<IReadOnlyCollection<AccessRequest>> GetAllAsync();
    Task<AccessRequest?> GetByIdAsync(Guid id, bool trackChanges = false);
    Task<bool> PendingRequestExistsAsync(Guid userId, Guid roleId);
    void Add(AccessRequest accessRequest);
    Task SaveChangesAsync();
}
