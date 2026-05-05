using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public class ProvisioningService(OpenIgaDbContext dbContext, IAuditService auditService) : IProvisioningService
{
    public async Task<bool> RoleExistsAsync(Guid roleId)
    {
        return await dbContext.Roles.AnyAsync(role => role.Id == roleId);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId)
    {
        return await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == userId && userRole.RoleId == roleId);
    }

    public async Task<ServiceResult> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid? performedBy)
    {
        var userExists = await dbContext.Users.AnyAsync(user => user.Id == userId);
        var roleExists = await RoleExistsAsync(roleId);
        if (!userExists || !roleExists)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "User or role was not found.");
        }

        var alreadyAssigned = await UserHasRoleAsync(userId, roleId);
        if (alreadyAssigned)
        {
            return ServiceResult.Success();
        }

        dbContext.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await dbContext.SaveChangesAsync();
        await auditService.LogAsync(AuditAction.RoleAssignedToUser, performedBy, userId);

        return ServiceResult.Success();
    }
}
