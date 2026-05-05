using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public class RoleService(
    OpenIgaDbContext dbContext,
    IAuditService auditService,
    ICurrentUserService currentUserService) : IRoleService
{
    public async Task<IReadOnlyCollection<RoleDto>> GetRolesAsync()
    {
        return await dbContext.Roles
            .AsNoTracking()
            .Select(role => role.ToDto())
            .ToListAsync();
    }

    public async Task<RoleDto?> GetRoleAsync(Guid id)
    {
        var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == id);
        return role?.ToDto();
    }

    public async Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<RoleDto>.Failure(ServiceError.Validation, "Role name is required.");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return ServiceResult<RoleDto>.Success(role.ToDto());
    }

    public async Task<ServiceResult> AssignPermissionAsync(Guid roleId, AssignPermissionRequest request)
    {
        var roleExists = await dbContext.Roles.AnyAsync(role => role.Id == roleId);
        var permissionExists = await dbContext.Permissions.AnyAsync(permission => permission.Id == request.PermissionId);
        if (!roleExists || !permissionExists)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "Role or permission was not found.");
        }

        var alreadyAssigned = await dbContext.RolePermissions.AnyAsync(rolePermission =>
            rolePermission.RoleId == roleId && rolePermission.PermissionId == request.PermissionId);
        if (!alreadyAssigned)
        {
            dbContext.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = request.PermissionId });
            await dbContext.SaveChangesAsync();
            await auditService.LogAsync(AuditAction.RolePermissionAssigned, currentUserService.UserId, null);
        }

        return ServiceResult.Success();
    }
}
