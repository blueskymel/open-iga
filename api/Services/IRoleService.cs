using OpenIga.Api.Dtos;

namespace OpenIga.Api.Services;

public interface IRoleService
{
    Task<IReadOnlyCollection<RoleDto>> GetRolesAsync();
    Task<RoleDto?> GetRoleAsync(Guid id);
    Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleRequest request);
    Task<ServiceResult> AssignPermissionAsync(Guid roleId, AssignPermissionRequest request);
}
