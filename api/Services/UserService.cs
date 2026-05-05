using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public class UserService(OpenIgaDbContext dbContext, IAuditLogService auditLogService) : IUserService
{
    public async Task<IReadOnlyCollection<UserDto>> GetUsersAsync()
    {
        return await dbContext.Users
            .AsNoTracking()
            .Select(user => user.ToDto())
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserAsync(Guid id)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id);
        return user?.ToDto();
    }

    public async Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<UserDto>.Failure(ServiceError.Validation, "Email is required.");
        }

        var emailExists = await dbContext.Users.AnyAsync(user => user.Email == request.Email);
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

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        await auditLogService.LogAsync(AuditAction.UserCreated, request.PerformedBy, user.Id);

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<ServiceResult> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user is null)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "User was not found.");
        }

        user.Email = request.Email;
        user.Name = request.Name;
        user.Status = request.Status;

        await dbContext.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteUserAsync(Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user is null)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "User was not found.");
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> AssignRoleAsync(Guid userId, AssignRoleRequest request)
    {
        var userExists = await dbContext.Users.AnyAsync(user => user.Id == userId);
        var roleExists = await dbContext.Roles.AnyAsync(role => role.Id == request.RoleId);
        if (!userExists || !roleExists)
        {
            return ServiceResult.Failure(ServiceError.NotFound, "User or role was not found.");
        }

        var alreadyAssigned = await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == userId && userRole.RoleId == request.RoleId);
        if (alreadyAssigned)
        {
            return ServiceResult.Success();
        }

        dbContext.UserRoles.Add(new UserRole { UserId = userId, RoleId = request.RoleId });
        await dbContext.SaveChangesAsync();
        await auditLogService.LogAsync(AuditAction.RoleAssignedToUser, request.PerformedBy, userId);

        return ServiceResult.Success();
    }
}
