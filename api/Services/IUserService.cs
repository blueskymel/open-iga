using OpenIga.Api.Dtos;

namespace OpenIga.Api.Services;

public interface IUserService
{
    Task<IReadOnlyCollection<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(Guid id);
    Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserRequest request);
    Task<ServiceResult> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<ServiceResult> DeleteUserAsync(Guid id);
    Task<ServiceResult> AssignRoleAsync(Guid userId, AssignRoleRequest request);
    Task<ServiceResult<IReadOnlyCollection<EffectivePermissionDto>>> GetEffectivePermissionsAsync(Guid userId);
}
