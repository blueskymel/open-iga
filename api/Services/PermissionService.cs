using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public class PermissionService(OpenIgaDbContext dbContext) : IPermissionService
{
    public async Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync()
    {
        return await dbContext.Permissions
            .AsNoTracking()
            .Select(permission => permission.ToDto())
            .ToListAsync();
    }

    public async Task<PermissionDto?> GetPermissionAsync(Guid id)
    {
        var permission = await dbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == id);
        return permission?.ToDto();
    }

    public async Task<ServiceResult<PermissionDto>> CreatePermissionAsync(CreatePermissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<PermissionDto>.Failure(ServiceError.Validation, "Permission name is required.");
        }

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        dbContext.Permissions.Add(permission);
        await dbContext.SaveChangesAsync();

        return ServiceResult<PermissionDto>.Success(permission.ToDto());
    }
}
