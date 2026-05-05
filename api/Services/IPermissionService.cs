using OpenIga.Api.Dtos;

namespace OpenIga.Api.Services;

public interface IPermissionService
{
    Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync();
    Task<PermissionDto?> GetPermissionAsync(Guid id);
    Task<ServiceResult<PermissionDto>> CreatePermissionAsync(CreatePermissionRequest request);
}
