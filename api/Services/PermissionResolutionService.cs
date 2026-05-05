using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;

namespace OpenIga.Api.Services;

public class PermissionResolutionService(OpenIgaDbContext dbContext) : IPermissionResolutionService
{
    public async Task<IReadOnlyCollection<EffectivePermissionDto>> GetEffectivePermissionsAsync(Guid userId)
    {
        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId)
            .Join(
                dbContext.RolePermissions.AsNoTracking(),
                userRole => userRole.RoleId,
                rolePermission => rolePermission.RoleId,
                (userRole, rolePermission) => rolePermission.PermissionId)
            .Join(
                dbContext.Permissions.AsNoTracking(),
                permissionId => permissionId,
                permission => permission.Id,
                (permissionId, permission) => new EffectivePermissionDto(permission.Id, permission.Name))
            .Distinct()
            .OrderBy(permission => permission.Name)
            .ToListAsync();
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
    {
        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId)
            .Join(
                dbContext.RolePermissions.AsNoTracking(),
                userRole => userRole.RoleId,
                rolePermission => rolePermission.RoleId,
                (userRole, rolePermission) => rolePermission.PermissionId)
            .Join(
                dbContext.Permissions.AsNoTracking(),
                permissionId => permissionId,
                permission => permission.Id,
                (permissionId, permission) => permission)
            .AnyAsync(permission => permission.Name == permissionName);
    }
}
