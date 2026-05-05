using OpenIga.Api.Models;

namespace OpenIga.Api.Dtos;

public record UserDto(Guid Id, string Email, string? Name, UserStatus? Status, DateTime CreatedAt);

public record CreateUserRequest(string Email, string? Name, UserStatus? Status);

public record UpdateUserRequest(string Email, string? Name, UserStatus? Status);

public record AssignRoleRequest(Guid RoleId);
