using OpenIga.Api.Dtos;

namespace OpenIga.Api.Services;

public interface IPermissionResolutionService
{
    Task<IReadOnlyCollection<EffectivePermissionDto>> GetEffectivePermissionsAsync(Guid userId);
    Task<bool> HasPermissionAsync(Guid userId, string permissionName);
}
