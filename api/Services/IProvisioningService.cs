namespace OpenIga.Api.Services;

public interface IProvisioningService
{
    Task<bool> RoleExistsAsync(Guid roleId);
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId);
    Task<ServiceResult> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid? performedBy);
}
