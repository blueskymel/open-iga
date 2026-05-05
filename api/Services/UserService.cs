using OpenIga.Api.Dtos;
using OpenIga.Api.Models;
using OpenIga.Api.Repositories;

namespace OpenIga.Api.Services;

public class UserService(
    IUserRepository userRepository,
    IProvisioningService provisioningService,
    IAuditService auditService,
    ICurrentUserService currentUserService,
    IPermissionResolutionService permissionResolutionService) : IUserService
{
    public async Task<IReadOnlyCollection<UserDto>> GetUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(user => user.ToDto()).ToList();
    }

    public async Task<UserDto?> GetUserAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user?.ToDto();
    }

    public async Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<UserDto>.Failure(ServiceError.Validation, "Email is required.");
        }

        var emailExists = await userRepository.EmailExistsAsync(request.Email);
        if (emailExists)
        {
            return ServiceResult<UserDto>.Failure(ServiceError.Conflict, "A user with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            Status = request.Status ?? UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        userRepository.Add(user);
        await userRepository.SaveChangesAsync();
        await auditService.LogAsync(AuditAction.UserCreated, currentUserService.UserId, user.Id);

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<ServiceResult> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(id, trackChanges: true);
        if (user is null)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "User was not found.");
        }

        user.Email = request.Email;
        user.Name = request.Name;
        user.Status = request.Status;

        await userRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteUserAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id, trackChanges: true);
        if (user is null)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "User was not found.");
        }

        userRepository.Remove(user);
        await userRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> AssignRoleAsync(Guid userId, AssignRoleRequest request)
    {
        return await provisioningService.AssignRoleToUserAsync(userId, request.RoleId, currentUserService.UserId);
    }

    public async Task<ServiceResult<IReadOnlyCollection<EffectivePermissionDto>>> GetEffectivePermissionsAsync(Guid userId)
    {
        var userExists = await userRepository.ExistsAsync(userId);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyCollection<EffectivePermissionDto>>.Failure(
                ServiceError.NotFound,
                "User was not found.");
        }

        var permissions = await permissionResolutionService.GetEffectivePermissionsAsync(userId);
        return ServiceResult<IReadOnlyCollection<EffectivePermissionDto>>.Success(permissions);
    }
}
